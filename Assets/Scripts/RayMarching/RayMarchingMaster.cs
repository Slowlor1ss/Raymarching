using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RayMarchingMaster : MonoBehaviour
{
    struct ShapeData
    {
        public Vector3 position;
        public Vector3 scale;
        public Vector3 colour;
        public Shape.ShapeType shapeType;
        public Shape.Operation operation;
        public float blendStrength;
        public int numChildren;

        public static unsafe int GetSize()
        {
            return sizeof(ShapeData);
        }

        public ShapeData(Shape shape)
        {
            position = shape.Position;
            scale = shape.Scale;
            colour = shape.ColourVec3;
            shapeType = shape.shapeType;
            operation = shape.operation;
            blendStrength = shape.blendStrength * 3;
            numChildren = shape.numChildren;
        }
    }

    struct LightData
    {
        public Vector3 position;
        public float intensity;
        public float range;
        public Vector3 colour;

        public static unsafe int GetSize()
        {
            return sizeof(LightData);
        }

        public LightData(Light light)
        {
            position = light.transform.position;
            intensity = light.intensity;
            range = light.range;
            colour = new Vector3(light.color.r, light.color.g, light.color.b);
        }
    }

    [SerializeField]
    ComputeShader _computeShader;

    RenderTexture _renderTexture;
    Camera _camera;
    ComputeBuffer _shapeBuffer = null;
    ComputeBuffer _lightBuffer = null;

    void UpdateRenderTexture()
    {
        //check if render texture exists and is it still matched out camera; if not -> update
        if (_renderTexture == null 
            || _renderTexture.width != _camera.pixelWidth || _renderTexture.height != _camera.pixelHeight)
        {
            //delete old
            if (_renderTexture != null) 
                _renderTexture.Release();

            //create new updated version
            _renderTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _renderTexture.enableRandomWrite = true;
            _renderTexture.Create();
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture result)
    {
        _camera = Camera.current;
        UpdateRenderTexture();

        //passes all required scene info to GPU
        UpdateSceneInfo();

        _computeShader.SetTexture(0, "Source", source);
        _computeShader.SetTexture(0, "Result", _renderTexture);

        int threadGroupsX = Mathf.CeilToInt(_camera.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(_camera.pixelHeight / 8.0f);
        _computeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(_renderTexture, result);

        _shapeBuffer.Dispose();
        _lightBuffer.Dispose();
    }

    void UpdateSceneInfo()
    {
        PassShapesToComputeShader();

        _computeShader.SetMatrix("CameraToWorld", _camera.cameraToWorldMatrix);
        _computeShader.SetMatrix("CameraInverseProjection", _camera.projectionMatrix.inverse);

        //pass all light data to out GPU
        List<Light> Lights = new List<Light>(FindObjectsOfType<Light>());
        List<LightData> allLights = Lights.Select(x => new LightData(x)).ToList();
        _lightBuffer = new ComputeBuffer(allLights.Count, LightData.GetSize());
        _lightBuffer.SetData(allLights);
        _computeShader.SetBuffer(0, "Lights", _lightBuffer);
        _computeShader.SetInt("AmntLights", allLights.Count);
    }

    void PassShapesToComputeShader()
    {
        List<Shape> allShapes = new List<Shape>(FindObjectsOfType<Shape>());
        allShapes.Sort((a, b) => a.operation.CompareTo(b.operation));

        List<Shape> orderedShapes = new List<Shape>();

        foreach (var shape in allShapes)
        {
            if (shape.transform.parent != null) 
                continue;

            //find all top level shapes (those with no parent)
            Transform parentShape = shape.transform;
            orderedShapes.Add(shape);
            shape.numChildren = parentShape.childCount;

            //add children (for now doesn't support nested children)
            for (int i = 0; i < parentShape.childCount; ++i)
            {
                if (parentShape.GetChild(i).GetComponent<Shape>() != null)
                {
                    orderedShapes.Add(parentShape.GetChild(i).GetComponent<Shape>());
                    orderedShapes.Last().numChildren = 0;
                }
            }
        }

        ShapeData[] shapeData = new ShapeData[orderedShapes.Count];
        for (int i = 0; i < orderedShapes.Count; i++) 
            shapeData[i] = new ShapeData(orderedShapes[i]);

        //pass all shape data to out GPU
        _shapeBuffer = new ComputeBuffer(shapeData.Length, ShapeData.GetSize());
        _shapeBuffer.SetData(shapeData);
        _computeShader.SetBuffer(0, "Shapes", _shapeBuffer);
        _computeShader.SetInt("AmntShapes", shapeData.Length);
    }

}

# Raymarching
 
 
## How to open

To run this project you can simply open the assests folder it in the unity engine.<br>
<br>
Note: when opening the project you might get a notification saying there are error's in the project<br>
these errors are simply becouse of some project settings and is fairly easy to fix.<br><br>
![EnterSafeMode-png](Images/EnterSafeMode.png)<br><br>
In unity to get the size of struct using "sizeof" you have to mark the code as unsafe like so:<br>
```cpp
struct ShapeData
{
    public static unsafe int GetSize()
    {
        return sizeof(ShapeData);
    }
}
```
and that exact piece of code is whats causing the error when opening the file for the fist time.<br>
You can easly fix this by allowing unsafe code for that project like so:<br><br>
<img src="Images/UnsafeCodeExplained.png" style=" width:70% ; height:70% "><br>
After that all errors will be gone and<br>
I ensure you none of the code is actually "unsafe" ;)<br>



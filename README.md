# Raymarching
 
Note: when opening the project you might get a notification saying there are error's in the project
these errors are simply becouse of some project settings and is fairly easy to fix.
[error picture]
In unity to get the size of struct using "sizeof" you have to mark the code as unsafe like so:
```cpp
public static unsafe int GetSize()
{
    return sizeof(ShapeData);
}
```
and that exact pice of code is whats causing the error when opening the file for the fist time.
You can easly fix this by allowing unsafe code for that project like so:
[unsafe code picture]
After that all errors will be gone and
I ensure you none of the code is actually "unsafe" ;)



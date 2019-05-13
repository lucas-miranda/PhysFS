# PhysFS# Overview

*Still under development*

**Interop.cs**  

Maps every function from [*PhysFS/src/physfs.h*](/src/physfs.h), including original documentation, each of C# managed pointer data representation is signaled with comments.
This file itself can be used to manipulate PhysFS entirely, but all error checking must be done by yourself.

**PhysFS.cs**

Provides a mean to use PhysFS in an object-oriented way, each returning value is checked and proper exception is thrown when necessary.
All allocation and deallocation is made automatically, so you don't need to worry about it.

Some functions, like *PHYSFS_openWrite*, *PHYSFS_openAppend* and *PHYSFS_openRead*, operate in their own classes (**PhysFSFileWrite** and **PhysFSFileReader**, for example), so not every function at [*PhysFS/src/physfs.h*](/src/physfs.h) will be available on **PhysFS** class.

**IO/Stream/PhysFSFileWriter.cs**

The file write stream. Supporting append.

**IO/Stream/PhysFSFileReader.cs**

The file read stream.

**IO/PhysFSFile**

A *PHYSFS_File* representation. Close() must be called manually.

**Exceptions/PhysFSException.cs**

Standard PhysFS# exception, check the *ErrorCode* if catching and recovering from an error is intended.

**Util/**

Contains some helper methods and extensions.



Some minimal tests are provided at test folder. Contains some examples also.

# License

PhysFS# is under [MIT License](/csharp/LICENSE).

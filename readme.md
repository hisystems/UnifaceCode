Uniface Code
============

**WARNING: This is NOT production ready - use at your own risk**

Overview
--------

Uniface stores source code in the database and not in text files. This tool supports downloading the source code from the Uniface database into text files. In future, support will be added for uploading source code to the Uniface database and for a complete source control workflow (probably based around git initially) so that code changes can be tracked and managed.

Specifically, supports;

* Downloading to a .uni file:
    * Forms
    * Entities
    * Services
    * Message
    * Proc
    * Include Procs
* Tested with Uniface 9

To do:
* Uploading objects from a .uni file to a 

Getting Started
---------------

1. In VS Code run task: `Compile-UnifaceGetAllObjectIds-Release`
2. In VS Code run task: `Compile-UnifacePullObject-Release`
3. Setup the config file: `Init-Uniface.ps1 -databaseName "main" -databaseConnectionString "Server=localhost;Initial Catalog=DatabaseName;User Id=admin;Password=;" -codeRootDir "$env:homepath\source\UnifaceMain"`
4. Pull all objects from database: `Pull-Uniface.ps1` and open the files in a text editor like VS Code.
AND/OR 
5. Pull an object: `Pull-Object.ps1 -objectId "{Type}/Library/ObjectName"`.
    The object ID follows the folder structure. Specifically;
    Type = Form|Service|Message|Proc|IncludeProc

VS Code Support
---------------
Basic text highlighting is supported in VS Code here: http://github.com/hisystems/UnifaceLanguage

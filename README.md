# SINFONI
A modular middleware for transparent, efficient network communication in heterogeneous distributed infrastructures

## Third Party Libraries and Licenses

SINFONI uses the NuGet Package manager to manage dependencies on third party libraries. When you build SINFONI, make sure that NuGET is installed, and allow NuGet package restore on build in your IDE. More information on NuGet can be found here: https://www.nuget.org/

Currently, SINFONI uses the following NuGet packages with the following licenses:

Package | Version | License | License Text
--------|---------| --------| -------------
Dynamitey | 1.0.2 | Apache v2 | http://www.apache.org/licenses/LICENSE-2.0
Json.NET | 6.0.2 |  MIT | http://opensource.org/licenses/MIT
log4Net | 2.0.3 |  Apache v2 | http://logging.apache.org/log4net/license.html
NUnit | 2.6.4 | NUnit License | http://nunit.org/nuget/license.html
SuperSocket | 0.8 | Apache v2 | http://superwebsocket.codeplex.com/license
WebSocket4Net | 0.12 | Apache v2 | http://www.apache.org/licenses/LICENSE-2.0

## Project Structure

SINFONI is a highly modular project that provides several extension mechanisms which are also reflected in the project structure. The project is divided into the following parts:

* __Examples__ : Simple examples that show how to use SINFONI on server- and client side
* __Protocols__ : Ready-To-Use serialization protocol modules
* __SINFONI__ : The core implementation of the middleware
* __SINFONIUnitTests__ : A collection of test cases for the core implementation
* __Transports__ : Ready-To-Use transport modules
* 

## Quickstart Guide

### Build and use SINFONI

All information that is needed to build SINFONI from source is provided within the repository. The code is written to be compatible with .Net 4.0 . Simply open the solution file in the IDE of your choice. Enable automatic package restore on build by NuGet (http://www.nuget.org) and rebuild the entire solution.
The compiled libraries are then located in the _bin_ folders of the respective projects.

Link these compiled libraries (SINFONI.dll and the transport / protocol modules) you would like to use to your project. 

Please refer to the example projects __Examples/SimpleServer__ and __Examples/SimpleClient__ to see how to register transport and protocol modules to your application, start a SINFONI service on server side, and connect to the server from a client.

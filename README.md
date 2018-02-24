# Overview
This repository contains a simple socket server and client written in **C#**, **.Net Core 2.0** and **Socket**.


## UML diagram

This is the socket server diagram model

<img src="https://raw.githubusercontent.com/zabihy/SocketServer/master/diagram.png" width="350px">

<!-- graph TD -->
<!-- A[Client A] -- Request -- S((Server))-->
<!-- B[Client B] -- Request -- S((Server))-->
<!-- C[Client C] -- Request -- S((Server))-->
<!-- D[Client D] -- Request -- S((Server))-->
<!-- S -- Enqueue -- T(ActionBlock)-->
<!-- T -- Dequeue -- AGA(Agent A)-->
<!-- T -- Dequeue -- AGB(Agent B)-->
<!-- T -- Dequeue -- AGC(Agent C)-->
<!-- T -- Dequeue -- AGD(Agent D)-->
<!-- AGA -- Enqueue Result -- R(Result Queue)-->
<!-- AGB -- Enqueue Result -- R(Result Queue)-->
<!-- AGC -- Enqueue Result -- R(Result Queue)-->
<!-- AGD -- Enqueue Result -- R(TcpClient/Protocol Queue)-->
<!-- R -- Enqueue Result -- DEQ(TcpClient StreamWriter)-->
<!-- DEQ -- Response To Clients -- Clients(Clients A, B, C, D)-->

## Project Structure

 > **Note** The solution contains **three** projects. 
 - **SocketServer.Protocols** which is written in ***netstandard 2.0*** and contains Protocols and shared classes.
 - **SocketClient** which is responsible for running the client app and sending response to the socket server. 

- **SocketServer.AB** which is the socket server and uses ***ActionBlock*** for parallel processing the socket byte array and ***consumer/producer pattern*** to response to the clients.

## How to build

To build the project:
1. Change the ServerPort in program.cs in all projects.
2. Write click on the solution and choose **Set Startup Projects**.
3. Choose ***multiple startup projects***
4. Choose **SocketClient** and **SocketServer.AB** projects and choos ***Action*** as ***Start***.
5. Click Ok to close ***Solution Property Pages*** window.
6. Simply click **Start** to run the projects in ***Debug*** or ***Release*** mode.

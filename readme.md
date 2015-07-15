# Nautabus

## Overview

Sometimes you need the features of a service bus, but don't want to commit to the 
infrastructural complexities, management overhead, and price associated with most
traditional options like NServiceBus or Azure Service Bus. You just want a little
bus. 

Nautabus is a light-weight, hostable server that provides durable messaging
services between multiple client applications. It implements the Publish-Subscribe 
architectural design pattern over RESTful APIs and WebSockets using ASP.NET Web API 
and SignalR. 

Included is a convenient .Net class library for use within your own client 
applications.

## What is does

Nautabus clients publish and/or subscribe to message 'topics' hosted on the server. 
Published messages are stored and tracked on the server until all subscriptions have 
retrieved their messages. Messages may contain any content, so long as it can be 
serialized to JSON.

Nautabus is not designed to be a full-blown managed service bus implementation. It 
handles only topic based messaging, uses only HTTP and WebSocket protocols, and 
has minimal set of management, logging and diagnostic features. 

## Project Status

**Pre-Alpha**

This project is very new, and has not reached a release milestone yet. Currently 
it has working end-to-end pub/sub capabilities and a basic EF managed database.

The current focus is on server mechanisms to enhance the reliability of message 
delivery, and features to ensure that Nautabus server can be scaled 
horizontally to multiple instances.

## Components

#### Server

The Nautabus Server is implemented as a class library project. Build on ASP.NET 
Web API and SignalR, the server library must be embedded in another OWIN compatible 
host application. 

There are two sample host applications included in the project; a console application 
(self-host), and an IIS web application (web-host). 

You can embed the server component directly within your own application so long as 
you install the necessary OWIN components, and initialize CORS and SignalR. 

#### Nautabus Client

The Nautabus Client is a .Net class library that provides a thin set of abstractions 
for interacting with Nautabus Server. If you prefer, you can skip the Nautabus client 
and directly reference the standard SignalR client libraries instead. You can also 
leverage the standard SignalR JavaScript proxies too

## Developer Notes:

#### Delivery and Acknowledgement
When the client library receives a message, it will send an acknowledgment back to 
the server immediately, before it calls your message handler. 

Once delivered, it is the client's responsibility to make sure that the message is 
processes successfully. If there is a failure during processing, the client should 
handle and/or retry the operation internally.

#### Duplicate Delivery
It is possible that a client could receive more than one instance of the same message. 
While this should be an extreme rare case. The client would ideally make sure that 
operations performed against a message are [idempotent](http://stackoverflow.com/a/1077421/10115). 

When idempotence isn't an option, the client should track the messages IDs it has already 
processed and ignore any duplicates. The client wouldn't need to track IDs over a 
long period of time --typically, just tracking IDs in memory while the client is 
running is sufficient.

#### Delivery Order 
Messages are not guaranteed to arrive at a client in the same order in which they were 
published -such is the nature of an async system. Nautabus does not include features for 
enforcing message sequences or sub-grouping related messages within a topic. You 
can include sequence or group information within the messages, but it is up to the 
clients to understand and react to that information accordingly.

## Due Credit

The basic implementation used by Nautabus was inspired by 
[this article by Matt Honeycutt](http://trycatchfail.com/blog/post/Simple-Server-Client-Pub-Sub-using-SignalR.aspx)

The initial Visual Studio project stucture was borrowed from Darrel Miller's 
[SimpleApiTemplate on GitHub](https://github.com/darrelmiller/SimpleApiTemplate/), which he 
describes in more detail in [this article](http://www.bizcoder.com/the-simplest-possible-asp-net-web-api-template)

The data model and some of the SignalR code has been borrowed from earlier prototype versions of 
[TicketDesk](https://github.com/NullDesk).

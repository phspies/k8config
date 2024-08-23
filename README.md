# Overview

This project aims for showcase the usage of dotnet reflection in creating dynamic nested objects and walking those object with a text based editor to build out Kubernetes configuration files in memory. This utility also aims to show how the current kubernetes configuration specification can be converted into classes and then dynamicly created with the reflection capabilities of dotnet.

The project was created with a commandline window framework to make it usable on both Mac, Linux and Windows platforms.

# Project Details

Configurator Toolkit for Kubernetes Fling is a commandline-based tool for authoring Kubernetes YAML files and performing basic Kubernetes administration tasks. Written in .Net 5.0, the tool enables cross platform use on Windows, OSX and Linux.

The tool enables two main features which can be devided into two categories:

*   YAML Functionality
    *   YAML definition by means of a structured folder based approach.
    *   Import/Export YAML constructs with support for multiple YAML definitions in a single file.
    *   Realtime attribute type checking to ensure values entered matches the required Kubernetes API types.
    *   Interactivly create/edit/delete any attribute within the defined YAML construct.
    *   Verify YAML definition based on the Kubernetes API.
    *   Realtime access to API attribute documentation which is based on the Kubernetes API.
    *   Realtime view of the current Kubernetes defined service.
*   Realtime Management Functionality
    *   Easy connection access to already defined kubectl constructs as defined by the "kubectl" command.
    *   Additional connectivity options via a proxy API session.
    *   Track changes to namespaces, pods, deployment, replication sets and services in realtime mode.
    *   Report of changes to namespaces, pods, deployment, replication sets and services by means of Kubernetes events.
    *   Edit or apply any current of new Kubernetes definition construct with access to object configuration information and logs
    *   Execute commands to pods with access to output of these commands for troubleshooting
    
# Animated Gif

<img src="./ezgif-3-65215835e083.gif" width="100%"/>

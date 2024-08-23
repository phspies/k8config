# Overview

This project aims for showcase the usage of dotnet reflection in creating dynamic nested objects and walking those object with a text based editor to build out Kubernetes configuration files in memory. This utility also aims to show how the current kubernetes configuration specification can be converted into classes and then dynamicly created with the reflection capabilities of dotnet.

The project was created with a commandline window framework to make it usable on both Mac, Linux and Windows platforms.

# Project Details

    <p>Configurator Toolkit for Kubernetes Fling is a commandline-based tool for authoring Kubernetes YAML files and performing basic Kubernetes administration tasks. Written in .Net 5.0, the tool enables cross platform use on Windows, OSX and Linux.</p>
    <p>The tool enables two main features which can be devided into two categories:
        <ul>
            <li>
                YAML Functionality
                <ul>
                    <li>YAML definition by means of a structured folder based approach.</li>
                    <li>Import/Export YAML constructs with support for multiple YAML definitions in a single file.</li>
                    <li>Realtime attribute type checking to ensure values entered matches the required Kubernetes API types.</li>
                    <li>Interactivly create/edit/delete any attribute within the defined YAML construct.</li>
                    <li>Verify YAML definition based on the Kubernetes API.</li>
                    <li>Realtime access to API attribute documentation which is based on the Kubernetes API.</li>
                    <li>Realtime view of the current Kubernetes defined service.</li>
                </ul>
            </li>
            <li>
                Realtime Management Functionality
                <ul>
                    <li>Easy connection access to already defined kubectl constructs as defined by the "kubectl" command.</li>
                    <li>Additional connectivity options via a proxy API session.</li>
                    <li>Track changes to namespaces, pods, deployment, replication sets and services in realtime mode.</li>
                    <li>Report of changes to namespaces, pods, deployment, replication sets and services by means of Kubernetes events.</li>
                    <li>Edit or apply any current of new Kubernetes definition construct with access to object configuration information and logs</li>
                    <li>Execute commands to pods with access to output of these commands for troubleshooting</li>
                </ul>
            </li>

        </ul>
    </p>

# Animated Gif

<img src="./ezgif-3-65215835e083.gif" width="100%"/>

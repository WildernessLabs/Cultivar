<img src="Design/wildernesslabs-meadow-cultivar.jpg"  alt="Meadow.ProjectLab, C#, iot" style="margin-bottom:10px" />

# Cultivar

Greenhouse Management Meadow Solution

## Contents
* [Overall Concept](#overall-concept)
* [Meadow Application](#getting-started)
* [HMI reTerminal Application](#hardware-specifications)
* [Additional Samples](#additional-samples)
* [Support](#support)

## Overall Concept

The concept to automate a basic greenhouse, consists on controlling four peripherals:

* A large cylindrical live bulb to give the appropriate amount of light for the plants health growth throughout the entire year.
* Sprinkler system to water the plants and keep the soil moist.
* Heater and Ventilation fan to control and maintain an appropriate room temperature.

Here's a sketch of Cultivar:

![](Design/wildernesslabs-meadow-cultivar-sketch.jpg)

On the top right we have a project lab which is in charge of controlling the whole system, and all four peripherals mentioned above are connected to it, some indirectly with a relay module in the middle to turn on or off things like the heater and the ventilation fan.

Not only the Project Lab is controlling these four peripherals, it's using its onboard BME688 environmental sensor to check the greenhouse's temperature, and soil moisture sensors to control how much the plants need to be watered.

Finally, the Project Lab is connected to a WiFi and periodically sending data from the sensors and the current status of all its peripherals over to Meadow.Cloud, which in turn, sends them to a Digital Twin on Microsoft Azure.

Having a virtual representation of the Project Lab in an Azure Digital Twin lets you monitor and control your Greenhouse from anywhere on the planet using any application that connects to it, which in this case, we built an HMI app using Avalonia on a reTerminal by SeeedStudios.

## Meadow Application

![](Design/wildernesslabs-cultivar-projectlab.jpg)

Lorem Ipsum

## HMI reTerminal Application

![](Design/wildernesslabs-cultivar-reterminal.jpg)

Lorem Ipsum

## Additional Samples

1. **[Setup your Meadow Build Environment](http://developer.wildernesslabs.co/Meadow/Getting_Started/Deploying_Meadow/)** - If you haven't deployed a Meadow app before, you'll need to setup your IDE extension(s), deploy Meadow.OS, etc.
2. **[Run the Demo App](Source/ProjectLab_Demo)** - Deploy the Project Lab demonstration app to see the built in peripherals at work.
3. **[Check out the Project Lab Samples](https://github.com/WildernessLabs/Meadow.ProjectLab.Samples)** - We recommend cloning the [Meadow.ProjectLab.Samples](https://github.com/WildernessLabs/Meadow.ProjectLab.Samples) repo. There you'll find a bunch of awesome samples that you can run right out-of-the box! 
<a href="https://github.com/WildernessLabs/Meadow.ProjectLab.Samples">
    <img src="Design/project-lab-samples.png" alt="project-lab, iot, project, samples" style="margin-top:10px;margin-bottom:10px" />
</a>

## Support

Having trouble building/running these projects? 
* File an [issue](https://github.com/WildernessLabs/Meadow.Desktop.Samples/issues) with a repro case to investigate, and/or
* Join our [public Slack](http://slackinvite.wildernesslabs.co/), where we have an awesome community helping, sharing and building amazing things using Meadow.

# servirtium-demo-dotnet-climate-tck

Demonstration project for Servirtium .NET implementation (https://github.com/servirtium/servirtium-dotnet)

This is roughly the sme as the Java example - https://github.com/servirtium/demo-java-climate-tck - but following idioms for .NET

The climate API tested uses a simple programmatic wrapper for World Bank's climate-data service. It can respond to requests with XML or 
JSON payloads, and the `Servirtium.AspNetCore` module can record and payback either. This is a standard showcase for Servirtium.

## Notable source files:

Climate API demo class: [Servirtium.Demo/ClimateApi.cs](https://github.com/servirtium/sdemo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApi.cs). 

Servirtium in use:

* Playback of a Servirtium recording: [Servirtium.Demo/ClimateApiPlaybackTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApiPlaybackTests.cs) (reuses ClimateApiTests.cs - see below)
* Making a Servirtium recording: [Servirtium.Demo/ClimateApiRecordingTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApiRecordingTests.cs) (reuses ClimateApiTests.cs - see below)
* For contrast, direct tests against the climate service (no Servirtium): [Servirtium.Demo/ClimateApiTests.cs](https://github.com/servirtium/demo-dotnet-climate-tck/blob/master/Servirtium.Demo/ClimateApiTests.cs) 

For your own use of Servirtium, you'd do something like the record and playback tests.
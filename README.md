# Controller Tester
*Copyright 2014 Alvin Stanescu, Software Engineering Chair @ Technische Universitaet Munchen*

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

This software is provided with no liability whatsoever and is an Alpha version, therefore **use AT YOUR OWN RISK!**

This software is based on the work *"Automated Model-in-the-Loop Testing of Continuous Controllers Using Search"*
by *Reza Matinnejad, Shiva Nejati, Lionel C. Briand, Thomas Bruckmann and Claude Poull.*
A digital copy is available from http://dx.doi.org/10.1007/978-3-642-39742-4_12.

## Requirements
To be able to compile the Controller Tester, the following software is needed:
* **Visual Studio 2013** or newer, with .NET Framework 4.5.1 (however, Visual Studio 2012 with .NET Framework 4.5 should also work if the target framework of the solution is changed)
* The MahApps.Metro UI Framework (found on NuGet)
  
To be able to run the Controller Tester, the following software is required:
* a PC with Windows Vista SP2, Windows 7 SP1, Windows 8 or Windows 8.1 with the *.NET Framework 4.5.1* installed
* **MATLAB 2011a or newer**, with Simulink, the Parallelization Toolbox, Optimization Toolbox and Global Optimization Toolbox

## Usage 
The Controller Tester can manage test projects and is able to use MATLAB Simulink to manually or automatically simulate controller models. Fault models are used as a basis for describing the problems which occurr when implementing a particular system. The currently used version of MATLAB depends on the version registered as an Automation Server. To change the registered COM Automation Server, type the following command into the MATLAB Console of your chosen version:
> !matlab -regserver

More information is available at: http://www.mathworks.com/help/matlab/call-matlab-com-automation-server.html

The currently implemented fault models are:

### The Step Fault Model
The Step Fault Model was described by *Mattinejad et. al.* (see above). The automatic test case generation process attempts to maximize the objective function of a requirement in order to find a worst case scenario. The available requirements are:
* Stability
* Liveness
* Smoothness
* Responsiveness
* Oscillation

The particularity of this fault model is that the input signal, the controller's desired value, is a step function. The reasoning behind the fault model is that usually, in a real-life scenario, the controller does not start from 0 (or whatever initial value it may have). Because of this, a real-life scenario of the controller would be one where the initial value is arbitrary (the current actual value) rather than a constant value. To simulate this scenario, two desired values are generated - an initial desired value and a final desired value, and the controller is simulated with each value for half of the time. This scenario also shows the controller's performance in the case of negative calculations (especially in the case of undershoots), which are typically neglected when doing Model-in-the-Loop testing.

The objective functions of each requirement are computed based on the controller's actual value signal after the final desired value is input to it. The intermediate outcome of the test case generation process is a heatmap indicating possible problematic regions in the 2-D input space. The user can choose the regions he wants to investigate further. The final outcome of our test case generation is a worst case test scenario for a particular requirement in a certain region.

## Further development
The tool is designed to be easily extendible with plug-in Controller Fault Models. A template for Fault Models will be provided in a future version.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FM4CC.Environment;
using FM4CC.ExecutionEngine;
using FM4CC.ExecutionEngine.Matlab;
using FM4CC.FaultModels;
using FM4CC.FaultModels.TwoSteps;
using System.IO;

namespace FM4CC.Test
{
    [TestClass]
    public class PosNegFaultModelTests
    {
        static ExecutionEnvironment engine;
        static ExecutionInstance instance;
        static FaultModel fm;
        // Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject
        [ClassInitialize]
        public static void Setup(TestContext tc)
        {
            engine = new MatlabExecutionEngine();
            engine.AcquireProcess();

            instance = new ExecutionInstance("FMTest", "MATLAB");

            string solutionPath = Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
            string modelPath = solutionPath + "Resources\\DCMotorModel\\DC_Motor_Model.mdl";
            string modelSettingsPath = solutionPath + "Resources\\DCMotorModel\\DC_Motor_Settings.m";
            string configurationPath = solutionPath + "Resources\\dcmotorworkspace";

            instance.PutValue("SUTSettingsPath", modelSettingsPath);
            instance.PutValue("SUTPath", modelPath);
            instance.PutValue("ConfigurationPath", configurationPath);
            fm = new TwoStepsFaultModel(engine, new TwoStepsFaultModelConfiguration(configurationPath));

        }

        [ClassCleanup]
        public static void Cleanup()
        {
            engine.RelinquishProcess();
            engine.Dispose();
        }

        [TestMethod]
        public void SimulationTest()
        {
            string message;

            // Sets up the environment of the execution engine
            fm.ExecutionInstance = instance;
            fm.SetUpEnvironment();

            bool result = fm.TestFunctionality(out message);

            fm.TearDownEnvironment(false);

            Assert.IsTrue(message.Contains("success") && result);
        }

        [TestMethod]
        public void RandomExplorationTest()
        {
            // Sets up the environment of the execution engine
            fm.ExecutionInstance = instance;
            fm.SetUpEnvironment();

            string result = (string)fm.Run("RandomExploration");

            fm.TearDownEnvironment(false);

            Assert.IsTrue(result.Contains("success"));
        }
    }
}

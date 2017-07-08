This is the full source code to Mesh Maker VR.

NOTE: This is the development version of Mesh Maker VR, not the production version. We were in the middle of a massive refactor when we decided to release MMVR as open source, so the code is a lot cleaner than the production code. However, reference images and alignment tools are broken. We were in the middle of re-implementing these as 2.0 versions with additional features. Everything else should work as expected.

# Dependencies

Unity Asset Store Dependencies:
* ProBuilder (free version - I'm pretty sure we're not using this, but it may be required by the scene)
* VR Gimbal Camera (required by the scene, but not really necessary. Feel free to remove it.)
* SteamVR
* Simple Color Picker
* UnityTestTools (necessary to run integration tests)

  Note that UnityTestTools is slightly broken in modern Unity. Fix it by deleting the test that contains ExpectedException:
```diff
diff -urwb Assets/UnityTestTools/Examples/UnitTestExamples/Editor/SampleTests.cs "../Mesh Maker VR/Assets/UnityTestTools/Examples/UnitTestExamples/Editor/SampleTests.cs"
--- Assets/UnityTestTools/Examples/UnitTestExamples/Editor/SampleTests.cs       2016-09-12 09:38:58.000000000 -0400
+++ "../Mesh Maker VR/Assets/UnityTestTools/Examples/UnitTestExamples/Editor/SampleTests.cs"    2017-04-17 10:51:57.137423000 -0400
@@ -70,13 +70,6 @@
         {
         }

-        [Test]
-        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "expected message")]
-        public void ExpectedExceptionTest()
-        {
-            throw new ArgumentException("expected message");
-        }
-
         [Datapoint]
         public double zero = 0;
         [Datapoint]
```

* The Amazing Wireframe Shader (Note that this takes 30 minutes to compile on a 6700k i7 with 32 gb of ram. I have no idea why. It's a good asset otherwise.)

  Note that I've included a patch to The Amazing Wireframe Shader that adds face normals. To install:
```bash
patch -p2 --binary < VacuumShaders_Add_Face_Normals.patch
```
* TheLabRenderer (I'm pretty sure we're not using this, but it may be required by the scene)
* Free Sunset skybox (skyb1)

This project also requires the CreateThis VR UI project. It's available [here](https://github.com/createthis/createthis_vr_ui) on github. Just copy over the Assets/CreateThis folder into the Assets directory.

# Licensing

The included code is MIT Licensed. However, the integration tests use a Pikachu model with the following license:

Low-Poly Pikachu by FLOWALISTIK is licensed under the Creative Commons - Attribution - Non-Commercial - Share Alike license.

# Chat

Slack channel: [meshmakervr](https://meshmakervr.slack.com) [(Join link)](https://meshmakervr.slack.com/signup)

# Patreon

Love this project and want to help it be successful? Support us on Patreon, today: [Mesh Maker VR Patreon Page](https://www.patreon.com/createthis)

NOTE: Before this project was open sourced it was making about $348 per month on Steam. If we reach that goal in Patreon pledges development will continue unchanged.

This is the full source code to Mesh Maker VR.

NOTE: This is the development version of Mesh Maker VR, not the production version. We were in the middle of a massive refactor when we decided to release MMVR as open source, so the code is a lot cleaner than the production code. However, reference images and alignment tools are broken. We were in the middle of re-implementing these as 2.0 versions with additional features. Everything else should work as expected.

# Dependencies

This project currently uses Unity 5.6.2f1. Get it here: [https://store.unity.com](https://store.unity.com)

Unity Asset Store Dependencies:
* [ProBuilder](https://www.assetstore.unity3d.com/#!/content/11919?aid=1100l35sb) (free version - I'm pretty sure we're not using this, but it may be required by the scene)
* [VR Gimbal Camera](https://www.assetstore.unity3d.com/#!/content/92219?aid=1100l35sb) (required by the scene, but not really necessary. Feel free to remove it.)
* [SteamVR](https://www.assetstore.unity3d.com/#!/content/32647?aid=1100l35sb)
* [Simple Color Picker](https://www.assetstore.unity3d.com/#!/content/7353?aid=1100l35sb)
* [UnityTestTools](https://www.assetstore.unity3d.com/#!/content/13802?aid=1100l35sb) (necessary to run integration tests)

  Note that UnityTestTools is slightly broken in modern Unity. Fix it by deleting the test that contains ExpectedException. I've included a patch to do this quickly and easily from git bash on windows:
```bash
patch -p2 < UnityTestTools.patch
```

* [The Amazing Wireframe Shader](https://www.assetstore.unity3d.com/#!/content/18794?aid=1100l35sb) (Note that this takes 30 minutes to compile on a 6700k i7 with 32 gb of ram. I have no idea why. It's a good asset otherwise.)

  Note that I've included a patch to The Amazing Wireframe Shader that adds face normals. To install:
```bash
patch -p2 --binary < VacuumShaders_Add_Face_Normals.patch
```
* [TheLabRenderer](https://www.assetstore.unity3d.com/#!/content/63141?aid=1100l35sb) (I'm pretty sure we're not using this, but it may be required by the scene)
* [Free Sunset skybox](https://www.assetstore.unity3d.com/#!/content/4183?aid=1100l35sb) (skyb1)

This project also requires the CreateThis VR UI project. It's available [here](https://github.com/createthis/createthis_vr_ui) on github. Just copy over the Assets/CreateThis folder into the Assets directory.

# Licensing

The included code is MIT Licensed. However, the integration tests use a Pikachu model with the following license:

Low-Poly Pikachu by FLOWALISTIK is licensed under the Creative Commons - Attribution - Non-Commercial - Share Alike license.

# Chat

Slack channel: [meshmakervr](https://meshmakervr.slack.com) [(Join link)](https://meshmakervr.slack.com/signup)

# Patreon

Love this project and want to help it be successful? Support us on Patreon, today: [Mesh Maker VR Patreon Page](https://www.patreon.com/createthis)

NOTE: Before this project was open sourced it was making about $348 per month on Steam. If we reach that goal in Patreon pledges development will continue unchanged.

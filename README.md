# SuperSmartMERGE

Original code by [StreamingAlchemy](https://github.com/StreamingAlchemy/StreamingAlchemy)

You can find the original script [here](https://github.com/StreamingAlchemy/StreamingAlchemy/blob/main/SA-S02E21/SmartMERGE.vb)

Description from the [YouTube video]((https://www.youtube.com/watch?v=AOCXovJK5e8)) explaining the original script;
`The MERGE Transition in vMix is a great way to create visual dynamics by letting elements change size and position right on screen. It is frequently used to bring a person in a multi-box shot full screen, and then push them back into a multi-box when they finish. However, the visual quality of the Merge depends on the ordering of the layers in the MultiView.`

The SmartMERGE script dynamically adjusts the order of layers to create a better-looking MERGE transition. However, this only works when going from a layered multi-view input to a single non-layed input. I have modified the script and added in new logic to allow a SmartMERGE between two layered multi-view inputs, I call this... **SuperSmartMERGE**

It is fairly robust, but there will no doubt be some situations where it fails. Always test it out with your planned transitions to check that it works for your situation.

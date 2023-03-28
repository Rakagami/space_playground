# Space Playground

This is a collection of Unity3D scenes that help me understand space stuff better

## Quick Start

Clone this project and use [Unity3D](https://store.unity.com/#plans-individual) to open this project. As of writing this README.md, I'm using Unity `2021.3.8f1`.

## Scene: RotationalFrames

When dealing with rotations and coordinates in space it can get quite messy. There is [ECIs, ECEFS](https://en.wikipedia.org/wiki/Earth-centered_inertial) for position. In terms of rotations, can be the reference frame of **body coordinates**, **orbital coordinates**, and **inertial coordinates**. I wanted to preview the difference between these three systems as a visualization.

https://user-images.githubusercontent.com/44206941/228184460-06290c33-22d4-458b-b82f-21ac40c6a45b.mp4

This scene can be found under `RotationalFrames.unity`

## Scene: ConstellationOrbits

Satellite constellations can be a bit weird to visualize, at least for me. In this scene, I visualize circular orbit constellations (proportion-wise LEO constellations) that supports multiple shells, and the parameters of `inclination`, `amount of orbital planes`, and a [Walker Delta / Walker Star](https://en.wikipedia.org/wiki/Satellite_constellation) distinction.

https://user-images.githubusercontent.com/44206941/228184264-4d5702b4-e089-46ae-90f0-103f8b1c1030.mp4

This scene can be found under `ConstellationOrbits.unity`

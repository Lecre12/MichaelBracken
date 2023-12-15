using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using GameNetcodeStuff;

namespace HEHE.Patch
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class MigerSound
    {
        static bool lookingToFlowerMan = false;
        static AudioClip newSFX;
        static ManualLogSource mls;
        static Vector3 flowerManPositionLastSaw = new Vector3();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void prepareAudio()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource("Lecre.HEHEMod");
            // Load the audio file
            string location = ((BaseUnityPlugin)HeheBase.instance).Info.Location;
            string text = "HEHE.dll";
            string text2 = location.TrimEnd(text.ToCharArray());
            string path = text2 + "Miger.wav";
            ((MonoBehaviour)HeheBase.instance).StartCoroutine(LoadAudio("file:///" + path, clip =>
            {
                newSFX = clip;
            }));
        }

        static IEnumerator LoadAudio(string url, Action<AudioClip> callback)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    mls.LogError("Failed to load audio assets!");
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip == null)
                    {
                        mls.LogError("The audio clip is null after loading!");
                    }
                    else
                    {
                        callback(clip);
                        mls.LogInfo("Audio inserted");
                    }
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void migerSoundPatch(ref PlayerControllerB __instance)
        {

            if (HeheSound.flowerMan != null)
            {
                FlowermanAI flowerman = HeheSound.flowerMan;
                Vector3 flowerManPosition = flowerman.serverPosition;
                PlayerControllerB playerRef = __instance;
                bool reproduced = false;

                if (playerRef.HasLineOfSightToPosition(flowerManPosition) && !lookingToFlowerMan)
                {
                    flowerManPositionLastSaw = flowerManPosition;
                    lookingToFlowerMan = true;
                }
                else if (lookingToFlowerMan && playerRef.HasLineOfSightToPosition(flowerManPositionLastSaw) && !reproduced)
                {
                    reproduced = true;
                    AudioSource audioSource = playerRef.gameObject.AddComponent<AudioSource>();
                    audioSource.clip = newSFX;
                    audioSource.Play();
                    mls.LogInfo("El jugador ha perdido la visión del Flowerman, es hora de esconderse ;)");
                }

                if (!playerRef.HasLineOfSightToPosition(flowerManPosition))
                {
                    lookingToFlowerMan = false;
                    reproduced = false;
                }
            }

        }
    }
}

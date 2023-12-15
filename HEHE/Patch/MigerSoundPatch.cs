using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using GameNetcodeStuff;
using System.Security.Policy;
using System.Threading;

namespace HEHE.Patch
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class MigerSound
    {
        static bool lookingToFlowerMan = false;
        static AudioClip newSFX;
        static ManualLogSource mls;

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
            if(HeheSound.flowerMan != null)
            {
                Vector3 flowerManPosition = new Vector3();
                PlayerControllerB playerRef = __instance;
                if (playerRef.HasLineOfSightToPosition(HeheSound.flowerMan.serverPosition) && !lookingToFlowerMan)
                {
                    lookingToFlowerMan = true;
                    flowerManPosition = HeheSound.flowerMan.serverPosition;
                }
                if(playerRef.HasLineOfSightToPosition(flowerManPosition) && flowerManPosition != null)
                {    
                    AudioSource audioSource = playerRef.gameObject.AddComponent<AudioSource>();
                    audioSource.clip = newSFX;
                    audioSource.Play();
                    mls.LogInfo("Someone saw a Flowerman what a scary incident, its time to hide ;)");
                }
                else if(!playerRef.HasLineOfSightToPosition(flowerManPosition) && flowerManPosition != null)
                {
                    lookingToFlowerMan = false;
                }
            }
            
        }

    }
}

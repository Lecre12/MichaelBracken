using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;


namespace HEHE.Patch
{
    [HarmonyPatch(typeof(FlowermanAI))]
    internal class HeheSound
    {
        static bool done = false;
        static AudioClip newSFX;
        static ManualLogSource mls;
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void prepareAudio()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource("Lecre.conchaSuMareMod");
            // Load the audio file
            string location = ((BaseUnityPlugin)HeheBase.instance).Info.Location;
            string text = "HEHE.dll";
            string text2 = location.TrimEnd(text.ToCharArray());
            string path = text2 + "HEHE.wav";
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
        static void FlowerManAudioKillPatch(ref FlowermanAI __instance)
        {
            FlowermanAI flowerMan = __instance;
            if (flowerMan.inKillAnimation && !done)
            {
                done = true;
                AudioSource audioSource = flowerMan.gameObject.AddComponent<AudioSource>();
                audioSource.clip = newSFX;
                audioSource.Play();
                mls.LogInfo("The FlowerMan got someone HEHE");
            }
            if (!flowerMan.inKillAnimation)
            {
                done = false;
            }
        }
        
    }
}

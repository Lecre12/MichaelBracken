using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;


namespace HEHE.Patch
{
    [HarmonyPatch(typeof(FlowermanAI))]
    internal class HeheSound
    {
        public static FlowermanAI killingFlowerMan;
        static AudioClip newSFX;
        static ManualLogSource mls;
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void prepareAudio()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource("Lecre.MichaelBrackenMod");
            mls.LogInfo("Starting to upload audio");
            // Load the audio file
            if (HeheBase.instance != null)
            {
                string location = ((BaseUnityPlugin)HeheBase.instance).Info.Location;
                string text = "MichaelBracken.dll";
                string text2 = location.TrimEnd(text.ToCharArray());
                string path = text2 + "HEHEsound.wav";
                ((MonoBehaviour)HeheBase.instance).StartCoroutine(LoadAudio("file:///" + path, clip =>
                {
                    newSFX = clip;
                }));
            }
            else
            {
                mls.LogWarning("Instance was readed like <null> retriying to load audio files");
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string modPath = Path.GetDirectoryName(assemblyLocation);
                string soundPath = Path.Combine(modPath, "HEHEsound.wav");
                mls.LogInfo("(else) IS THE AUDIO FILE LOCATED HERE??: " + soundPath);
                CoroutineHelper.StartCoroutine(LoadAudio("file:///" + soundPath, sound =>
                {
                    newSFX = sound;
                }));
            }
            
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
            if (__instance.inKillAnimation && killingFlowerMan == null)
            {
                killingFlowerMan = __instance;
                AudioSource audioSource = __instance.gameObject.AddComponent<AudioSource>();
                audioSource.clip = newSFX;
                audioSource.Play();
                mls.LogInfo("The Bracken got someone HEHE");
            }
            if (!__instance.inKillAnimation && __instance == killingFlowerMan)
            {
                killingFlowerMan = null;
            }
        }
        
    }
}

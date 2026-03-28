using UnityEngine;

namespace DomsExpandedIngredientsAndEffects.Utilities
{
    public static class WavUtility
    {
        public static AudioClip ToAudioClip(byte[] data, string name)
        {
            try
            {
                int channels    = data[22] | (data[23] << 8);
                int sampleRate  = data[24] | (data[25] << 8) | (data[26] << 16) | (data[27] << 24);
                int bitDepth    = data[34] | (data[35] << 8);
                int dataStart   = 44;

                for (int i = 12; i < data.Length - 4; i++)
                {
                    if (data[i] == 'd' && data[i+1] == 'a' && data[i+2] == 't' && data[i+3] == 'a')
                    {
                        dataStart = i + 8;
                        break;
                    }
                }

                int sampleCount = (data.Length - dataStart) / (bitDepth / 8);
                float[] samples = new float[sampleCount];

                if (bitDepth == 16)
                {
                    for (int i = 0; i < sampleCount; i++)
                    {
                        short s = (short)(data[dataStart + i * 2] | (data[dataStart + i * 2 + 1] << 8));
                        samples[i] = s / 32768f;
                    }
                }
                else if (bitDepth == 8)
                {
                    for (int i = 0; i < sampleCount; i++)
                        samples[i] = (data[dataStart + i] - 128) / 128f;
                }

                AudioClip clip = AudioClip.Create(name, sampleCount / channels, channels, sampleRate, false);
                clip.SetData(samples, 0);
                return clip;
            }
            catch (System.Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[WavUtility] Failed to parse {name}: {ex.Message}");
                return null;
            }
        }
    }
}
#if MONO
using ScheduleOne.Audio;
using ScheduleOne.DevUtilities;
#else

using Il2CppScheduleOne.Audio;
using Il2CppScheduleOne.DevUtilities;
#endif

namespace DomsExpandedIngredientsAndEffects.Utilities
{
    public static class SoundHelper
    {
        public static float SFXVolume
        {
            get
            {
                try
                {
                    var am = Singleton<AudioManager>.Instance;
                    return am != null ? am.GetVolume(EAudioType.FX) : 1f;
                }
                catch { return 1f; }
            }
        }

        public static float MusicVolume
        {
            get
            {
                try
                {
                    var am = Singleton<AudioManager>.Instance;
                    return am != null ? am.GetVolume(EAudioType.Music) : 1f;
                }
                catch { return 1f; }
            }
        }
    }
}
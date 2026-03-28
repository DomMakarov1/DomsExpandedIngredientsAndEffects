using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;

#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class SingleEventUpsetEffect : Effect
    {
        private static GameObject _bsodCanvas;
        private static bool       _active;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            if (_active) return;
            _active = true;
            MelonLoader.MelonCoroutines.Start(ShowBSOD());
        }

        public override void ClearFromPlayer(Player player)
        {
            _active = false;
            if (_bsodCanvas != null)
                UnityEngine.Object.Destroy(_bsodCanvas);
        }

        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }

        private static IEnumerator ShowBSOD()
        {
            _bsodCanvas = new GameObject("BSOD_Overlay");
            UnityEngine.Object.DontDestroyOnLoad(_bsodCanvas);

            Canvas canvas = _bsodCanvas.AddComponent<Canvas>();
            canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            _bsodCanvas.AddComponent<CanvasScaler>();
            _bsodCanvas.AddComponent<GraphicRaycaster>();

            GameObject bg = new GameObject("BG");
            bg.transform.SetParent(_bsodCanvas.transform, false);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.0f, 0.18f, 0.62f);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            AddText(_bsodCanvas, ":(",
                new Vector2(0.08f, 0.82f), new Vector2(0.5f, 0.95f),
                120, FontStyle.Normal);

            AddText(_bsodCanvas,
                "Your PC ran into a problem and needs to restart.\nWe're just collecting some error info, and then we'll restart for you.",
                new Vector2(0.08f, 0.62f), new Vector2(0.92f, 0.80f),
                28, FontStyle.Normal);

            AddText(_bsodCanvas,
                "100% complete",
                new Vector2(0.08f, 0.54f), new Vector2(0.5f, 0.62f),
                26, FontStyle.Normal);

            AddText(_bsodCanvas,
                "For more information about this issue and possible fixes, visit\nhttps://www.windows.com/stopcode\n\nIf you call a support person, give them this info:\nStop code: SINGLE_EVENT_UPSET\nWhat failed: MoonDust.sys",
                new Vector2(0.08f, 0.20f), new Vector2(0.92f, 0.52f),
                22, FontStyle.Normal);

            GameObject qr = new GameObject("QR");
            qr.transform.SetParent(_bsodCanvas.transform, false);
            Image qrImg = qr.AddComponent<Image>();
            qrImg.color = Color.white;
            RectTransform qrRect = qr.GetComponent<RectTransform>();
            qrRect.anchorMin = new Vector2(0.75f, 0.05f);
            qrRect.anchorMax = new Vector2(0.92f, 0.20f);
            qrRect.offsetMin = Vector2.zero;
            qrRect.offsetMax = Vector2.zero;

            yield return new WaitForSeconds(10f);

            _active = false;
            if (_bsodCanvas != null)
                UnityEngine.Object.Destroy(_bsodCanvas);
        }

        private static void AddText(GameObject parent, string content,
            Vector2 anchorMin, Vector2 anchorMax,
            int fontSize, FontStyle style)
        {
            GameObject obj = new GameObject("Text");
            obj.transform.SetParent(parent.transform, false);

            Text t = obj.AddComponent<Text>();
            t.text      = content;
            t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize  = fontSize;
            t.fontStyle = style;
            t.color     = Color.white;
            t.alignment = TextAnchor.UpperLeft;

            RectTransform r = obj.GetComponent<RectTransform>();
            r.anchorMin = anchorMin;
            r.anchorMax = anchorMax;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class SingleEventUpsetEffectDef : CustomEffect
    {
        public override string ID              => "singleeventupset";
        public override string Name            => "Single Event Upset";
        public override Color  Color           => new Color(0.0f, 0.18f, 0.62f);
        public override float  Addictiveness   => 0.40f;
        public override int    ValueChange     => 30;
        public override float  ValueMultiplier => 1.2f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<SingleEventUpsetEffect>();
    }
}
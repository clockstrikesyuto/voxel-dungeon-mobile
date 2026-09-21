using UnityEngine;

namespace VoxelDungeon.Core
{
    public static class UiFontProvider
    {
        private static Font cached;

        public static Font Get()
        {
            if (cached != null)
                return cached;

            string[] preferred =
            {
                "Yu Gothic UI",
                "Yu Gothic",
                "Meiryo UI",
                "Meiryo",
                "Hiragino Sans",
                "Hiragino Kaku Gothic ProN",
                "Noto Sans CJK JP",
                "Noto Sans JP",
                "Arial Unicode MS",
                "Arial"
            };

            string[] installed = Font.GetOSInstalledFontNames();

            foreach (string candidate in preferred)
            {
                foreach (string actual in installed)
                {
                    if (actual == candidate)
                    {
                        cached = Font.CreateDynamicFontFromOSFont(actual, 32);
                        if (cached != null)
                            return cached;
                    }
                }
            }

            cached = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return cached;
        }
    }
}

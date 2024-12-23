using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class DiscordLink
{
    public static string DISCORD_LINK = "https://discord.gg/9tnKg9XPpV";

    [MenuItem("Window/Open FactoryFramework Discord")]
    public static void OpenDiscord()
    {
        Application.OpenURL(DISCORD_LINK);
    }

    public static Button CreateDiscordButton()
    {
        var button = new Button(() => Application.OpenURL(DiscordLink.DISCORD_LINK))
        {
            text = "FactoryFramework Discord"
        };
        // new hex color #5865F2
        button.style.marginTop = 10;
        button.style.fontSize = 14;
        button.style.backgroundColor = new Color(0.345f, 0.396f, 0.949f);
        return button;
    }

}

using System.IO;
using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Core;
using SwarmUI.Text2Image;
using SwarmUI.Utils;

namespace SwarmVAEReflectionExtension;

public class SwarmVAEReflection : Extension
{
    public static T2IRegisteredParam<bool> VAEReflectionEnabled;

    public static T2IParamGroup VAEReflectionGroup;

    public override void OnPreInit()
    {
        string NodeDirectory = Path.Join(FilePath, "VAEReflectionNodes");
        ComfyUISelfStartBackend.CustomNodePaths.Add(NodeDirectory);
        Logs.Init($"Added {NodeDirectory} to the custom nodes paths");
    }

    public override void OnInit()
    {
        ExtensionName = "VAE Reflection";
        Version = "1.0.0";
        ExtensionAuthor = "Jelosus2";
        Description = "Allows VAEs to use reflection padding mode in their conv layers.";
        License = "MIT";
        Tags = ["parameters"];

        Logs.Init("Initializing VAE Reflection extension...");

        VAEReflectionGroup = new("VAE Reflection", Toggles: false, Open: false, IsAdvanced: true, OrderPriority: 8);
        VAEReflectionEnabled = T2IParamTypes.Register<bool>(new("Enable VAE Reflection",
            "Enables reflection padding mode in the conv layers for the VAE loaded.\nNote: It can cause issues if the VAE wasn't trained with reflection padding.",
            "false", Group: VAEReflectionGroup));

        WorkflowGenerator.AddModelGenStep(g =>
        {
            if (!g.UserInput.TryGet(VAEReflectionEnabled, out bool isEnabled) || !isEnabled)
                return;

            string node = g.CreateNode("AddReflectionToVAE", new JObject()
            {
                ["vae"] = g.LoadingVAE
            });
            g.LoadingVAE = [node, 0];
        }, -13);

        Logs.Init("VAE Reflection extension initialized.");
    }
}
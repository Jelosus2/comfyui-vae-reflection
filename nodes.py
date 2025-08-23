from torch import nn
import copy

class AddReflectionToVAE:
    @classmethod
    def INPUT_TYPES(cls):
        return {
            "required": {
                "vae": ("VAE", {"tooltip": "The VAE model to apply reflection to."})
            }
        }
    RETURN_TYPES = ("VAE",)
    OUTPUT_TOOLTIPS = ("The VAE with reflection applied.",)
    FUNCTION = "apply_reflection"
    CATEGORY = "utils"
    DESCRIPTION = "Adds reflection to a VAE model."

    def apply_reflection(self, vae):
        if vae.first_stage_model is None:
            return (vae,)

        vae_out = copy.copy(vae)
        vae_out.first_stage_model = copy.deepcopy(vae.first_stage_model)

        for module in vae_out.first_stage_model.modules():
            if isinstance(module, nn.Conv2d):
                pad_h, pad_w = module.padding if isinstance(module.padding, tuple) else (module.padding, module.padding)
                if pad_h > 0 or pad_w > 0:
                    module.padding_mode = "reflect"
        
        return (vae_out,)

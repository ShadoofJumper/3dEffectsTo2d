using System.Collections.Generic;
using System.ComponentModel;
using Animation;
using UnityEngine;

namespace Scipts.AnimationEditor
{
    [System.Serializable]
    public class EntityAnimationClip
    {
        public AnimationDirectionClips animations;
        public string clipName;
    }
    
    [System.Serializable]
    public class AnimationDirectionClips
    {
        public Sprite[] N;
        public Sprite[] NE;
        public Sprite[] S;
        public Sprite[] SE;
    }

    
    [CreateAssetMenu(fileName = "NewSpriteSteetData", menuName = "Art/animationSpritesheetData", order = 1)]
    public class AnimationSpritesheetSo : ScriptableObject
    {
        [Description("'EntityAnimation' must be on parent of all SpriteRenderers. So he can auto found all references.")]
        [Header("Animation sprites (first clip will be default):")]
        [SerializeField] private List<EntityAnimationClip> _animationClipDescriptions = new List<EntityAnimationClip>();

        public Sprite GetDefaultIdleSprite()
        {
            if (_animationClipDescriptions.Count > 0 
                && _animationClipDescriptions[0].animations.S.Length > 0)
            {
                return _animationClipDescriptions[0].animations.S[0];
            }
            
            Debug.LogError("Cant get default sprite for animation!");
            return null;
        }
        
        public Sprite[] GetAnimationSprites(string clipName, AnimationDirection animationDirection)
        {
            foreach (var animationClipDescription in _animationClipDescriptions)
            {
                if(animationClipDescription.clipName == clipName)
                {
                    switch (animationDirection)
                    {
                        case AnimationDirection.N:
                            return animationClipDescription.animations.N;
                        case AnimationDirection.NE:
                            return animationClipDescription.animations.NE;
                        case AnimationDirection.S:
                            return animationClipDescription.animations.S;
                        case AnimationDirection.SE:
                            return animationClipDescription.animations.SE;
                    }
                }
            }

            return null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scipts.AnimationEditor;
using UnityEngine;
namespace Animation
{
    public enum AnimationDirection
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }
    
    public class CharacterAnimation : MonoBehaviour
    {
        [SerializeField] private CharacterBase _characterBase;
        [SerializeField] private AnimationSpritesheetSo _animationSpritesheetData;
        [Header("Paramms:")] 
        [SerializeField] private int _frameRate = 16;
        private float _vertMoveScale = .45f;
        private IEnumerator _coroutineClip;
        private IEnumerator _coroutineUpdateAnimationMoveDirection;
        
        private SpriteRenderer[] _entitySpriteRenderers;
        private AnimationDirection _currentAnimationDirection = AnimationDirection.S;
        
        //{ "N","NE","E","SE","S","SW","W","NW" };
        private readonly AnimationDirection[] _animationDirections = new[]
        {
            AnimationDirection.N,
            AnimationDirection.NE,
            AnimationDirection.E,
            AnimationDirection.SE,
            AnimationDirection.S,
            AnimationDirection.SW,
            AnimationDirection.W,
            AnimationDirection.NW
        };

        private bool _isSpriteFlippedX = false;

        private List<Sprite> _currentClipPlay;
        private float _lastTime;

        private bool _isPlayAnim = false;
        private string _currentClipName;
        private bool _isLoop = true;

        private Vector3 _lastPos;
        private float _nextUpdateDirTime = 0.0f;
        public float _updateTimeDelay = 0.2f;

        private CharacterBase _positionComponent;

        private void Awake()
        {
            _entitySpriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            SetupAnimation(_characterBase);
        }
        
        public void SetupAnimation(CharacterBase character)
        {
            _positionComponent = character;
            Play("Idle");
            character.OnEnterState += OnEnterState;
            character.OnExitState += OnExitState;
        }
        
        private void OnEnterState(string newState)
        {
            if (newState == "Idle")
            {
                Play("Idle");
            }else if (newState == "Die")
            {
                Play("Die", false);
            }else if (newState == "Move")
            {
                Play("Move");
                StartUpdateAnimationMoveDirection();
            }
            else if (newState == "Attack")
            {
                Play("Attack", false);
                StartUpdateAnimationMoveDirection();
            }
        }

        private void OnExitState(string newState)
        {
            if (newState == "Move")
            {
                StopUpdateAnimationMoveDirection();
            }
        }

        #region Animation behavior

        private void StartUpdateAnimationMoveDirection()
        {
            _coroutineUpdateAnimationMoveDirection = UpdateAnimationSpriteMove();
            StartCoroutine(_coroutineUpdateAnimationMoveDirection);
        }

        private void StopUpdateAnimationMoveDirection()
        {
            if(_coroutineUpdateAnimationMoveDirection != null)
                StopCoroutine(_coroutineUpdateAnimationMoveDirection);
        }

        private IEnumerator UpdateAnimationSpriteMove()
        {
            while (true)
            {
                if (Time.time > _nextUpdateDirTime ) {
                    _nextUpdateDirTime += _updateTimeDelay;
                    if (_lastPos != transform.position)
                    {
                        Vector3 dir = _positionComponent.ViewDirection;
                        SetAnimationDirection(dir);
                        _lastPos = transform.position;
                    }
                }
                yield return null;
            }
        }

        private void RotateSpriteToAttack(Vector3 targetPosition)
        {
            Vector3 rotateDir = (targetPosition - transform.position).normalized;
            SetAnimationDirection(rotateDir);
        }

        #endregion

        #region Testing
        public void ChangeAnimationFrameRate(int frameRate)
        {
            _frameRate = frameRate;
        }
        /*
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 pointA = _positionComponent.position;
            Vector3 pointB = _positionComponent.position + _positionComponent.ViewDirection * 2f;
            Gizmos.DrawLine(pointA, pointB);
        }
        */
        #endregion
        
        private void Update()
        {
            UpdateAnimationSpriteTick();
        }

        private void Play(string clipName, bool isLoop = true)
        {
            Sprite[] spriteClip = GetAnimationSpriteClip(clipName);
            if (spriteClip != null)
            {
                _currentClipName = clipName;
                _isPlayAnim = true;
                _isLoop = isLoop;
                _lastTime = Time.time;
                _currentClipPlay = spriteClip.ToList();
            }
        }

        private void PlayTrigger(string clipName, Action onComplete = null)
        {
            Stop();
            _coroutineClip = PlayClip(clipName, onComplete);
            StartCoroutine(_coroutineClip);
        }

        private IEnumerator PlayClip(string clipName, Action onComplete = null)
        {
            Sprite[] spriteClip = GetAnimationSpriteClip(clipName);
            _lastTime = Time.time;

            int totalFrames = 0;
            while (totalFrames < spriteClip.Length)
            {
                float playTime = Time.time - _lastTime;
                totalFrames = (int)(playTime * _frameRate);
                int frame = totalFrames % spriteClip.Length;
                foreach (var sr in _entitySpriteRenderers)
                {
                    sr.sprite = spriteClip[frame];
                }
                yield return null;
            }
            onComplete?.Invoke();
        }

        private void UpdateAnimationSpriteTick()
        {
            if (!_isPlayAnim)
                return;
            
            float playTime = Time.time - _lastTime;
            int totalFrames = (int)(playTime * _frameRate);
            int frame = totalFrames % _currentClipPlay.Count;
            if (!_isLoop && totalFrames >= _currentClipPlay.Count)
            {
                frame = _currentClipPlay.Count-1;
                // Debug.Log($"NO LOOP: {frame}");
            }
            foreach (var sr in _entitySpriteRenderers)
            {
                sr.sprite = _currentClipPlay[frame];
            }
        }
        
        private void SetAnimationDirection(Vector3 moveDir)
        {
            UpdateDirectionClip(moveDir);
            //Debug.Log($"Direction: {_currentAnimationDirection.ToString()}, Flip: {_isSpriteFlippedX}");
        }

        private void UpdateDirectionClip(Vector3 newDir)
        {
            AnimationDirection newAnimationDirection = GetAnimationDirection(newDir, out bool isNeedFlipAnim);
            FlipAnimationIfNeed(isNeedFlipAnim);
            if (newAnimationDirection != _currentAnimationDirection)
            {
                _currentAnimationDirection = newAnimationDirection;
                //restart clip
                Play(_currentClipName);
            }
        }

        private void FlipAnimationIfNeed(bool newFlipX)
        {
            if (_isSpriteFlippedX != newFlipX)
            {
                foreach (var sp in _entitySpriteRenderers)
                {
                    sp.flipX = newFlipX;
                }
                _isSpriteFlippedX = newFlipX;
                Vector3 temp = transform.localScale;
                transform.localScale = new Vector3(Math.Abs(temp.x),Math.Abs(temp.y),Math.Abs(temp.z));
            }
        }

        private Sprite[] GetAnimationSpriteClip(string clipName)
        {
            Sprite[] sprites = _animationSpritesheetData.GetAnimationSprites(clipName, _currentAnimationDirection);
            if (sprites == null)
            {
                Debug.LogError($"Cant get animation name: {clipName}!");
                return null;
            }
            return _animationSpritesheetData.GetAnimationSprites(clipName, _currentAnimationDirection);
        }

        private AnimationDirection GetAnimationDirection(Vector3 dir, out bool isNeedFlipAnim)
        {
            isNeedFlipAnim = false;
            AnimationDirection coordinateDirection = GetCoordinateDirection(dir);
            AnimationDirection animationDirection = coordinateDirection;
            
            if (coordinateDirection == AnimationDirection.SW)
            {
                animationDirection = AnimationDirection.SE;
                isNeedFlipAnim = true;
            }
            else if (coordinateDirection == AnimationDirection.NW)
            {
                animationDirection = AnimationDirection.NE;
                isNeedFlipAnim = true;
            }
            else if (coordinateDirection == AnimationDirection.W)
            {
                animationDirection = AnimationDirection.SE;
                isNeedFlipAnim = true;
            }
            else if (coordinateDirection == AnimationDirection.E)
            {
                animationDirection = AnimationDirection.SE;
            }

            /*
            //fix SE direction
            if (coordinateDirection == AnimationDirection.W || coordinateDirection == AnimationDirection.SW)
            {
                isNeedFlipAnim = false;
            }else if (coordinateDirection == AnimationDirection.SE || coordinateDirection == AnimationDirection.E)
            {
                isNeedFlipAnim = true;
            }
            */
            
            return animationDirection;
        }
        
        private AnimationDirection GetCoordinateDirection(Vector3 dir)
        {
            float angle = Angle360(dir, transform.up, transform.right);
            return _animationDirections[((int)((angle + 22.5f) / 45.0f)) & 7];
        }
        
        private float Angle360(Vector3 dir, Vector3 entityForward, Vector3 entityRight)
        {
            var angle = Vector3.Angle(dir, entityForward);
            float angle2 = Vector3.Angle(dir, entityRight);
 
            if (angle2 > 90)
            {
                angle = 360 - angle;
            }

            return angle;
        }
        
        private void Stop()
        {
            if (_coroutineClip != null)
                StopCoroutine(_coroutineClip);
            StopUpdateAnimationMoveDirection();
            _isPlayAnim = false;
        }

        private void OnDestroy()
        {
            if (_coroutineClip != null)
                StopCoroutine(_coroutineClip);
            StopUpdateAnimationMoveDirection();
        }
    }
}

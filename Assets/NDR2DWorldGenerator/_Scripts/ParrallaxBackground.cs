using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NDR2DWorldGenerator
{
    public class ParrallaxBackground : MonoBehaviour
    {
        [SerializeField] private Vector2 parallaxEffectMultiplaier;
        [SerializeField] private bool infiniteHorizontal;
        [SerializeField] private bool infiniteVertical;

        private Transform cam;
        private Vector3 lastCamPos;
        float textureUnitSizeX;
        private float textureUnitSizeY;

        private void Start()
        {
            cam = Camera.main.transform;
            lastCamPos = cam.transform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
        }

        private void LateUpdate()
        {
            Vector3 deltaMove = cam.position - lastCamPos;
            transform.position += new Vector3( deltaMove.x * parallaxEffectMultiplaier.x, deltaMove.y * parallaxEffectMultiplaier.y, 0);
            lastCamPos = cam.position;

            if (infiniteHorizontal)
            {
                if (Mathf.Abs(cam.position.x - transform.position.x) >= textureUnitSizeX)
                {
                    float offsetPositionX = (cam.position.x - transform.position.x) % textureUnitSizeX;
                    transform.position = new Vector3(cam.position.x + offsetPositionX, transform.position.y);
                }
            }

            if (infiniteVertical)
            {
                if (Mathf.Abs(cam.position.y - transform.position.y) >= textureUnitSizeY)
                {
                    float offsetPositionY = (cam.position.y - transform.position.y) % textureUnitSizeY;
                    transform.position = new Vector3(transform.position.x, cam.position.y + offsetPositionY);
                }
            }
        }
    }
}
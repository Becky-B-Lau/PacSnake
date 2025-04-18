using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprites : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int aimationFrame { get; private set; }
    public bool loop = true;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        if (!this.spriteRenderer.enabled)
        {
            return;
        }

        this.aimationFrame++;
        if (this.aimationFrame >= this.sprites.Length && this.loop)
        {
            this.aimationFrame = 0;
        }

        if (this.aimationFrame >= 0 && this.aimationFrame < this.sprites.Length)
        {
            this.spriteRenderer.sprite = this.sprites[this.aimationFrame];
        }
    }

    public void Restart()
    {
        this.aimationFrame = -1;

        Advance();
    }
}

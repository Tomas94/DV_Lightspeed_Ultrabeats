using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private float _controlX, _controlY;
    Vector2 _dragDirection;
    Vector2 _screenBounds;

    private void Update()
    {
        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        MobileInputs();
    }

    void MobileInputs()
    {
        if (Input.touchCount < 1) return;

        Touch firstTouch = Input.touches[0];
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(firstTouch.position);

        switch (firstTouch.phase)
        {
            case TouchPhase.Began:
                _controlX = touchPos.x - transform.position.x;
                _controlY = touchPos.y - transform.position.y;
                break;
            case TouchPhase.Moved:
                transform.position = new Vector2(touchPos.x - _controlX,touchPos.y - _controlY);
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, (_screenBounds.x * -1), _screenBounds.x), Mathf.Clamp(transform.position.y, (_screenBounds.y * -1), _screenBounds.y), 0);
                break;
            case TouchPhase.Ended:
                transform.position = transform.position;
                break;
        }
    }
}

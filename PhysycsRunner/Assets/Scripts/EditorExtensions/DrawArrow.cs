using UnityEngine;

public static  class DrawArrow
{
    public static void ForGizmo(Vector3 pos, Vector3 endPos, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        ForGizmo(pos, endPos, Gizmos.color, arrowHeadLength, arrowHeadAngle);
    }
 
    public static void ForGizmo(Vector3 pos, Vector3 endPos, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(pos, endPos);

        var direction = (endPos - pos).normalized;
       
        var right = Quaternion.LookRotation (direction) * Quaternion.Euler (arrowHeadAngle, 0, 0) * Vector3.back;
        var left = Quaternion.LookRotation (direction) * Quaternion.Euler (-arrowHeadAngle, 0, 0) * Vector3.back;
        var up = Quaternion.LookRotation (direction) * Quaternion.Euler (0, arrowHeadAngle, 0) * Vector3.back;
        var down = Quaternion.LookRotation (direction) * Quaternion.Euler (0, -arrowHeadAngle, 0) * Vector3.back;
        
        Gizmos.DrawRay (endPos, right * arrowHeadLength);
        Gizmos.DrawRay (endPos, left * arrowHeadLength);
        Gizmos.DrawRay (endPos, up * arrowHeadLength);
        Gizmos.DrawRay (endPos, down * arrowHeadLength);
    }
 
    public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        ForDebug(pos, direction, Color.white, arrowHeadLength, arrowHeadAngle);
    }
    
    public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(pos, direction, color);
       
        var right = Quaternion.LookRotation (direction) * Quaternion.Euler (arrowHeadAngle, 0, 0) * Vector3.back;
        var left = Quaternion.LookRotation (direction) * Quaternion.Euler (-arrowHeadAngle, 0, 0) * Vector3.back;
        var up = Quaternion.LookRotation (direction) * Quaternion.Euler (0, arrowHeadAngle, 0) * Vector3.back;
        var down = Quaternion.LookRotation (direction) * Quaternion.Euler (0, -arrowHeadAngle, 0) * Vector3.back;
        
        Debug.DrawRay (pos + direction, right * arrowHeadLength, color);
        Debug.DrawRay (pos + direction, left * arrowHeadLength, color);
        Debug.DrawRay (pos + direction, up * arrowHeadLength, color);
        Debug.DrawRay (pos + direction, down * arrowHeadLength, color);
    }
}

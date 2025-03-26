using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Util
{
    public static Vector3 INVALID_VEC = new Vector3(-888888888, -888888888);

    public static bool MouseInCircle(Vector3 point, float radius) {
        Vector3 mouse = CameraController.Instance.GetMouseWorld();
        return (mouse.x -  point.x) * (mouse.x - point.x) + (mouse.y - point.y) * (mouse.y - point.y) <= radius * radius;
    }


    public static bool PointInCircle(Vector3 point, Vector3 circleCenter, float radius) {
        return (point.x - circleCenter.x) * (point.x - circleCenter.x) + (point.y - circleCenter.y) * (point.y - circleCenter.y) <= radius * radius;
    }

    public static float GetAnimationClipLength(Animator anim, string clipName) {
        var clips = anim.GetCurrentAnimatorClipInfo(0);
        for (int i = 0; i < clips.Length; i++) {
            if (clips[i].clip.name == clipName) {
                return clips[i].clip.length;
            }
        }
        return -1;
    }

    public static Vector3 ClampV3IntoRect(Vector3 v, Rect r) {
        return new Vector3(Mathf.Clamp(v.x, r.min.x, r.max.x), Mathf.Clamp(v.y, r.min.y, r.max.y));
    }

    public static T TryXTimes<T>(int maxTrys, System.Func<T> doFunc, System.Func<T, bool> whileFunc) {
        for (int i = 0; i < maxTrys; i++) {
            T t = doFunc();
            if (!whileFunc(t)) return t;
        }
        throw new GenerationFailedException();
    }

    public static Vector3 V2ToV3(Vector2 v2) {
        return v2;
    }

    public static int[] GetSceneIDByScenePrefix(string scenePrefix) {
        LinkedList<int> result = new LinkedList<int>();

        int maxScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < maxScenes; i++) {
            string sceneName = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string[] sceneNameSplit = sceneName.Split("/");
            sceneName = sceneNameSplit[sceneNameSplit.Length - 1];
            if (sceneName.ToLower().StartsWith(scenePrefix)) {
                result.AddLast(i);
            }
        }

        return result.ToArray();
    }

    public static void StopParticleSystemEmissionAndDestroy(ParticleSystem particleSystem, float destroyAfter) {
        particleSystem.Stop();
        Object.Destroy(particleSystem.gameObject, destroyAfter);
    }
}

public class DoEveryXFrame {
    private int x;
    private int currentFrame;
    private System.Action action;
    
    public DoEveryXFrame(int x, System.Action action) {
        this.x = x;
        this.currentFrame = x;

        this.action = action;
    }

    public bool UpdateFrame() {
        if (++currentFrame >= x) {
            action();
            currentFrame = 0;
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class Generator {
    public int amount;
    public float cooldown;

    private float currentCooldown;

    public Generator(int amount, float cooldown) {
        this.amount = amount;
        this.cooldown = cooldown;
    }

    public int Work(float deltaTime) {
        currentCooldown += deltaTime;
        if (currentCooldown > cooldown) {
            currentCooldown = 0;
            return amount;
        }
        return 0;
    }
}

public class GenerationFailedException : System.Exception {
}




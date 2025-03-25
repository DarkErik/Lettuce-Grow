using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnhancedTexts : MonoBehaviour
{
    private static readonly int TYPE_EVERY_X_FRAMES = 1;
    public const string WOBBLY = "wobbly", SCARY = "scary", RED = "red", GREEN = "green", BLUE = "blue", YELLOW = "yellow", RAINBOW = "rainbow";
    public static readonly string[] ALL_FORMATS = new string[] { WOBBLY, SCARY, RED, GREEN, BLUE, YELLOW, RAINBOW };
    [SerializeField] private TextMeshProUGUI textfield;
    [SerializeField] private float lineHeightScale = 100f;

    private LinkedList<FormatSegment> formatSegments = new LinkedList<FormatSegment>();

    private string text = null;
    private int textShownUntil;
    private bool isTyping = false;
    private DoEveryXFrame typeLetter;

    private void Awake() {
        if (text == null)
            SetText(textfield.text);
        typeLetter = new DoEveryXFrame(TYPE_EVERY_X_FRAMES, () => {
            textShownUntil++;
            textfield.maxVisibleCharacters = textShownUntil;
        });
    }


    public void SetTextTyping(string txt, bool startTyping = true) {
        formatSegments.Clear();
        this.text = ParseTxtRecursice(txt, 0);
        textfield.text = this.text;
        textShownUntil = 0;
        textfield.maxVisibleCharacters = 0;

        isTyping = startTyping;
    }

    public void SetText(string txt) {
        formatSegments.Clear();
        this.text = ParseTxtRecursice(txt, 0);
        textfield.text = txt;
        textShownUntil = txt.Length;
    }

    public void StartTyping() {
        isTyping = true;
    }

    public bool FinishedTyping() {
        return textShownUntil == text.Length;
    }

    public string ParseTxtRecursice(string txt, int index) {

        while (true) {
            index = txt.IndexOf("<", index);
            if (index == -1) {
                break;
            }
            int closing = txt.IndexOf(">", index);
            
            if (closing == -1) {
                break;
            }

            if (closing - index >= 10) {
                index = closing;
                continue;
            }
            string keyword = txt.Substring(index + 1, closing - index - 1);

            if (keyword.StartsWith("/")) {
                break;
            }

            FormatSegment segment = new FormatSegment();
            segment.start = index;
            foreach (string format in ALL_FORMATS) {
                if(keyword == format) {
                    segment.formatType = format;
                }
            }

            txt = txt.Remove(index, closing - index + 1);

            txt = ParseTxtRecursice(txt, index);

            int end = txt.IndexOf("</" + keyword + ">");
            if (end == -1) {
                break;
            }

            segment.end = end;

            txt = txt.Remove(end, keyword.Length + 3);
            
            

            formatSegments.AddLast(segment);

        }
        return txt;
    }

    public void Update() {

        if (isTyping && textShownUntil < text.Length) {
            typeLetter.UpdateFrame();
        }

        if (formatSegments.Count == 0) 
            return;

        textfield.ForceMeshUpdate();
        TMP_TextInfo textInfo = textfield.textInfo;

        float lineHeightWorldSpace = textInfo.lineInfo[0].lineHeight / 2048 * lineHeightScale * 86 * transform.lossyScale.y * textfield.fontSize;
        //Debug.Log(lineHeightWorldSpace);

        foreach (FormatSegment segment in formatSegments) {
            for(int i = segment.start; i < Mathf.Min(textShownUntil, segment.end); i++) {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                TMP_MeshInfo meshInfo = textInfo.meshInfo[charInfo.materialReferenceIndex];
                Vector3[] verts = meshInfo.vertices;


                for(int v = 0; v < 4; v++) {
                    int index = charInfo.vertexIndex + v;
                    Vector3 vertBefore = verts[index];

                    switch(segment.formatType) {
                        case WOBBLY: {
                                verts[index] = vertBefore + new Vector3(0, Mathf.Sin(Time.time * 3f + (vertBefore.x * 2 / lineHeightWorldSpace) * 0.5f) * lineHeightWorldSpace * 0.15f, 0);
                            break;
                            }
                        case SCARY:
                            verts[index] = vertBefore + new Vector3(Random.Range(-lineHeightWorldSpace, lineHeightWorldSpace) * 0.15f, Random.Range(-lineHeightWorldSpace, lineHeightWorldSpace) * 0.25f) * 0.2f;
                            break;
                        case RED:
                            meshInfo.colors32[index] = Color.red;
                            break;
                        case GREEN:
                            meshInfo.colors32[index] = new Color32(0, 8 * 16, 0, 255);
                            break;
                        case BLUE:
                            meshInfo.colors32[index] = Color.blue;
                            break;
                        case YELLOW:
                            meshInfo.colors32[index] = Color.yellow;
                            break;
                        case RAINBOW:
                            meshInfo.colors32[index] = Color.HSVToRGB(Mathf.Sin(vertBefore.x / lineHeightWorldSpace + Time.time * 2f) * 0.5f + 0.5f, 1, 1);
                            break;
                            
                        default:
                            //Debug.Log($"Umiplemented Format: {segment.formatType}");
                            break;

                    }
                }

                for (int j = 0; j < textInfo.meshInfo.Length; j++) {
                    meshInfo = textInfo.meshInfo[j];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    meshInfo.mesh.colors32 = meshInfo.colors32;
                    textfield.UpdateGeometry(meshInfo.mesh, j);
                }
            }
        }
    }

    private struct FormatSegment {
        public int start;
        public int end;
        public string formatType;
    }
}



using UnityEngine;
using System.Collections;
using System.IO;

public class Annotation : MonoBehaviour {
	public int pageWidth, pageHeight;
	public Collider page;
	public IIIFImageGet webdata;
	public RectTransform selection;

	private int sx, sy;
	private int w, h;
	private Vector2 originalStart;
	private bool annotating;
	void Update () {
		RaycastHit hit;
		if (page.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
			if (ButtonControls.current.getSelected () == ButtonControls.ANNOTATION_TOOL) {
				if (Input.GetMouseButtonDown (0)) {
					annotating = true;
					sx = (int)(pageWidth * hit.textureCoord.x);
					sy = (int)(pageHeight * hit.textureCoord.y);
					originalStart = Camera.main.WorldToScreenPoint (hit.point);
					selection.anchoredPosition = originalStart;
				} else if (Input.GetMouseButton (0)) {
					w = (int)(pageWidth * hit.textureCoord.x) - sx;
					h = (int)(pageHeight * hit.textureCoord.y) - sy;
					Vector3 sizeDelta = Camera.main.WorldToScreenPoint (hit.point) - (Vector3)originalStart;
					Vector2 newStart = selection.anchoredPosition;
					if (sizeDelta.x < 0) {
						newStart.x = originalStart.x + sizeDelta.x;
						sizeDelta.x = -sizeDelta.x;
					}
					if (sizeDelta.y < 0) {
						newStart.y = originalStart.y + sizeDelta.y;
						sizeDelta.y = -sizeDelta.y;
					}
					selection.anchoredPosition = newStart;
					selection.sizeDelta = sizeDelta;
				} else if (Input.GetMouseButtonUp (0)) {
					if (annotating) {
						if (w < 0) {
							sx += w;
							w = -w;
						}
						if (h < 0) {
							sy += h;
							h = -h;
						}
						annotating = false;
						StartCoroutine (MakeAnnotation (pageWidth - sx, sy, w, h));
					}
				}
			}
		} else if (!Input.GetMouseButton (0)) {
			annotating = false;
			selection.anchoredPosition = Vector2.zero;
			selection.sizeDelta = Vector2.zero;
		}
	}

	private IEnumerator MakeAnnotation(int x, int y, int w, int h){
		yield return ButtonControls.current.PopUp ();
		string anno = ButtonControls.current.getPopupText ();
		if (!anno.Equals ("")) {
			FileStream writter;
			string toWrite = ",\n";
			string filename = Application.persistentDataPath + "/anno.json";
			Debug.Log (filename);
			if (!File.Exists (filename)) {
				toWrite = "\n";
				File.WriteAllText (filename,
					"{\n\t\"@context\": \"http://www.shared-canvas.org/ns/context.json\","
					+ "\n\t\"@id\": \"" + filename + "\""
					+ ",\n\t\"@type\": \"sc:AnnotationList\",\n\t\"resources\": [\n\t\n\t]\n}\n");
			}
			toWrite += "\t\t{\n";
			toWrite += "\t\t\t\"id\": \"_:an" + System.DateTime.UtcNow.ToBinary().ToString () + "\",\n";
			toWrite += "\t\t\t\"@type\": \"oa:Annotation\",\n";
			toWrite += "\t\t\t\"resource\": {\n";
			toWrite += "\t\t\t\t\"id\": \"_:an" + System.DateTime.UtcNow.ToBinary().ToString () + "\",\n";
			toWrite += "\t\t\t\t\"@type\": \"cnt:ContentAsText\"\n";
			toWrite += "\t\t\t\t\"format\": \"text/plain\",\n";
			toWrite += "\t\t\t\t\"chars\": \"" + anno + "\",\n";
			toWrite += "\t\t\t\t\"language\": \"en\",\n";
			toWrite += "\t\t\t},\n";
			toWrite += "\t\t\t\"on\": \"" + webdata.webAddress + "#xywh=" + x.ToString() + "," + y.ToString() 
				+ "," + w.ToString() + "," + h.ToString() + "\"\n";
			toWrite += "\t\t}";

			writter = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			writter.Seek(0, SeekOrigin.End);
			while (writter.ReadByte() != ']')
				writter.Seek(-2, SeekOrigin.Current);
			writter.Seek(-5, SeekOrigin.Current);
			byte[] myBytes = System.Text.Encoding.Unicode.GetBytes (toWrite);
			writter.Write(myBytes,0,myBytes.Length);

			myBytes = System.Text.Encoding.Unicode.GetBytes ("\n\t]\n}\n");
			writter.Write(myBytes,0,myBytes.Length);

			writter.Close ();
		}
		selection.anchoredPosition = Vector2.zero;
		selection.sizeDelta = Vector2.zero;
	}
}

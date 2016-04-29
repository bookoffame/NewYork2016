using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

public class Annotation : MonoBehaviour {
	public int pageWidth, pageHeight;
	public Collider page;
	public IIIFImageGet webdata;
	public RectTransform selection;
	public Transform topLeft, bottomRight;

	private int sx, sy;
	private int w, h;
	private Vector2 originalStart, originalScreenStart;
	private bool annotating;

	void Update () {
		
		if (ButtonControls.current.getSelected () == ButtonControls.ANNOTATION_TOOL)
			MarkAnnotation ();
		
	}

	public ArrayList GetAnnotations(string data, string url){
		Regex regex = new Regex ("\"@type\": \"oa:Annotation\",(\\s|.)*?\"@type\": \"cnt:ContentAsText\"," +
			"(\\s|.)*?\"chars\": \"([^\"]*?)\",(\\s|.)*?\"on\": \""
			+ Regex.Escape(url) + "#xywh=(\\d*?),(\\d*?),(\\d*?),(\\d*?)\"");
		MatchCollection matches = regex.Matches (data);
		ArrayList list = new ArrayList ();
		foreach (Match m in matches) {
			AnnotationBox a;
			a.contents = m.Groups [3].ToString();
			a.x = (float)int.Parse(m.Groups [5].ToString())/pageWidth;
			a.y = (float)int.Parse(m.Groups [6].ToString())/pageHeight;
			a.w = (float)int.Parse(m.Groups [7].ToString())/pageWidth;
			a.h = (float)int.Parse(m.Groups [8].ToString())/pageHeight;
			list.Add (a);
		}
		return list;

	}

	private void MarkAnnotation(){
		RaycastHit hit;
		Vector3 topLeftCorr = Camera.main.WorldToScreenPoint (topLeft.position);
		Vector3 bottomRightCorr = Camera.main.WorldToScreenPoint (bottomRight.position);
		topLeftCorr.y = Screen.height - topLeftCorr.y;
		bottomRightCorr.y = Screen.height - bottomRightCorr.y;
		float myWidth = bottomRightCorr.x - topLeftCorr.x;
		float myHeight = bottomRightCorr.y - topLeftCorr.y;
		if (page.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 1000)) {
			Vector3 hitPoint = Input.mousePosition;
			hitPoint.y = Screen.height - hitPoint.y;
			if (Input.GetMouseButtonDown (0)) {
				annotating = true;
				sx = (int)(pageWidth * ((hitPoint.x - topLeftCorr.x)/myWidth));
				sy = (int)(pageHeight * ((hitPoint.y - topLeftCorr.y)/myHeight));
				originalStart = Camera.main.WorldToScreenPoint (hit.point);
				originalScreenStart = originalStart;
				originalScreenStart.y = Screen.height - originalScreenStart.y;
				selection.anchoredPosition = originalStart;
			} else if (Input.GetMouseButton (0)) {
				w = (int)(pageWidth * ((hitPoint.x - topLeftCorr.x)/myWidth)) - sx;
				h = (int)(pageHeight * ((hitPoint.y - topLeftCorr.y)/myHeight)) - sy;
				Vector3 sizeDelta = hitPoint - (Vector3)originalScreenStart;
				Vector2 newStart = selection.anchoredPosition;
				sizeDelta.y = -sizeDelta.y;
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
					StartCoroutine (MakeAnnotation (sx, sy, w, h));
				}
			}
		}
		if (!Input.GetMouseButton (0)) {
			annotating = false;
			selection.anchoredPosition = Vector2.zero;
			selection.sizeDelta = Vector2.zero;
		}
	}
	public string LocalAnnotationFile(){
		return  Application.persistentDataPath + "/anno.json";
	}

	private IEnumerator MakeAnnotation(int x, int y, int w, int h){
		yield return ButtonControls.current.PopUp ();
		string anno = ButtonControls.current.getPopupText ();
		if (!anno.Equals ("")) {
			FileStream writter;
			string toWrite = ",\n";
			string filename = LocalAnnotationFile ();
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
			toWrite += "\t\t\t\t\"@type\": \"cnt:ContentAsText\",\n";
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
			byte[] myBytes = System.Text.Encoding.ASCII.GetBytes (toWrite);
			writter.Write(myBytes,0,myBytes.Length);

			myBytes = System.Text.Encoding.ASCII.GetBytes ("\n\t]\n}\n");
			writter.Write(myBytes,0,myBytes.Length);

			writter.Close ();
		}
		selection.anchoredPosition = Vector2.zero;
		selection.sizeDelta = Vector2.zero;
		webdata.UpdateAnnotations ();
	}

	public struct AnnotationBox
	{
		public string contents;
		public float x,y,w,h;
	};
}

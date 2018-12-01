using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CubeController))]
public class LineController : MonoBehaviour
{
	[HideInInspector]
	public bool drawLine;
	public Text scoreText;
	public float lineThickness;

	struct customLine
	{
		public Vector3 StartPoint;
		public Vector3 EndPoint;
	};

	double score;
	double highScore;
	CubeController cube;
	List<Vector3> pointsList;
	LineRenderer line;
	bool isAlive;

	void Awake()
	{
		line = gameObject.AddComponent<LineRenderer>();
		line.material = new Material(Shader.Find("Particles/Additive"));
		line.positionCount = 0;
		line.startWidth = lineThickness;
		line.endWidth = lineThickness;
		line.startColor = Color.white;
		line.endColor = Color.white;
		line.useWorldSpace = true;
		drawLine = false;
		isAlive = true;
		cube = GetComponent<CubeController>();
		pointsList = new List<Vector3>();
		highScore = PlayerPrefs.GetFloat("highscore");
	}

	void Start()
	{
		if (highScore > 0) {
			scoreText.text = "SCORE " + System.Math.Round(score, 2) + "\n" + "HIGHSCORE " + System.Math.Round(highScore, 2);
		}
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(1)) {
			if (Input.GetMouseButton(0)) {
				drawLine = true;
			}
		}

		if (Input.GetMouseButtonUp(0)) {
			drawLine = false;
		}

		if (Input.GetMouseButtonUp(1)) {
			drawLine = false;

			if (isAlive) {
				ResetLine();
			}
		}

		if (drawLine && isAlive) {
			var position = transform.position;

			if (pointsList.Count == 0) {
				pointsList.Add(position);
				line.positionCount = pointsList.Count;
				line.SetPosition(pointsList.Count - 1, pointsList [pointsList.Count - 1]);
			}

			// Check minimum mouse distance from last point before adding a new point
			else if (Vector3.Distance(pointsList [pointsList.Count - 1], position) > 0.01f) {
				pointsList.Add(position);
				score = GetLineLenght();

				// Check if collides
				if (Collides()) {
					if (score > highScore) {
						highScore = score;
					}

					isAlive = false;
					drawLine = false;
					line.startColor = Color.red;
					line.endColor = Color.red;
					cube.HandleInputEnd(Input.mousePosition);
				}

				line.positionCount = pointsList.Count;
				line.SetPosition(pointsList.Count - 1, pointsList [pointsList.Count - 1]);

				// Update score and highscore text
				scoreText.text = "SCORE " + System.Math.Round(score, 2) + "\n" + "HIGHSCORE " + System.Math.Round(highScore, 2);
				PlayerPrefs.SetFloat("highscore", (float) highScore);
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "Plane" && drawLine == false) {
			ResetLine();
		}
	}

	bool Collides()
	{
		if (pointsList.Count < 3) return false;
		int TotalLines = pointsList.Count - 1;
		customLine [ ] lines = new customLine [TotalLines];

		// Fill lines array with past lines
		for (int i = 0; i < TotalLines; i++) {
			lines [i].StartPoint = pointsList [i];
			lines [i].EndPoint = pointsList [i + 1];
		}

		// Check the currentLine against every past line
		for (int i = 0; i < TotalLines - 2; i++) {
			customLine currentLine;
			currentLine.StartPoint = pointsList [pointsList.Count - 2];
			currentLine.EndPoint = pointsList [pointsList.Count - 1];
			if (LinesIntersect(lines [i], currentLine)) return true;
		}

		return false;
	}

	// Check whether given two line intersects
	bool LinesIntersect(customLine line1, customLine line2)
	{
		return ((Mathf.Max(line1.StartPoint.x, line1.EndPoint.x) >= Mathf.Min(line2.StartPoint.x, line2.EndPoint.x))
			&& (Mathf.Max(line2.StartPoint.x, line2.EndPoint.x) >= Mathf.Min(line1.StartPoint.x, line1.EndPoint.x))
			&& (Mathf.Max(line1.StartPoint.y, line1.EndPoint.y) >= Mathf.Min(line2.StartPoint.y, line2.EndPoint.y))
			&& (Mathf.Max(line2.StartPoint.y, line2.EndPoint.y) >= Mathf.Min(line1.StartPoint.y, line1.EndPoint.y)));
	}

	// Measure the lenght of the line
	double GetLineLenght()
	{
		double lenght = 0;
		int TotalLines = pointsList.Count - 1;

		if (TotalLines > 0) {
			for (int i = 0; i < TotalLines; i++) {
				lenght += Vector3.Distance(pointsList [i], pointsList [i + 1]);
			}
		}

		return lenght;
	}

	public void ResetLine()
	{
		isAlive = true;

		// Remove old line
		line.positionCount = 0;
		pointsList.RemoveRange(0, pointsList.Count);

		// Set its color back to white
		line.startColor = Color.white;
		line.endColor = Color.white;

		// Update score and highscore text
		score = 0;
		scoreText.text = "SCORE " + System.Math.Round(score, 2) + "\n" + "HIGHSCORE " + System.Math.Round(highScore, 2);
	}
}

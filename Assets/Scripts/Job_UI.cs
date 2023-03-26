using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Job_UI : MonoBehaviour
{
    public UIDocument CandidateDoc;
    public UIDocument JobDoc;
    public VisualTreeAsset jobItem;
    public Sprite[] JobImages;
    private Controller controller;
    private ScrollView job_ListView;
    private Button job_GoToCandidate;
    private Button job_Rematch;
    private Label job_Matches;
    private JobData[] jobDatas;

    void Start()
    {
        controller = GetComponent<Controller>();

        var root = JobDoc.rootVisualElement;
        job_ListView = root.Q<ScrollView>("Jobs");
        job_GoToCandidate = root.Q<Button>("GoToCand");
        job_Rematch = root.Q<Button>("Rematch");
        job_Matches = root.Q<Label>("Matches");

        job_GoToCandidate.clicked += OnGoToCandidate;
        job_Rematch.clicked += Rematch;
        OnGoToCandidate();
    }

    public void Rematch()
    {
        controller.Rematch();
    }

    public void OnGoToCandidate()
    {
        CandidateDoc.rootVisualElement.style.display = DisplayStyle.Flex;
        JobDoc.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void fillList(ExtendedJob[] jobs)
    {
        job_Matches.text = jobs.Length.ToString();
        job_ListView.Clear();
        foreach (var job in jobs)
        {
            var newJobEntry = jobItem.Instantiate();
            var newJobLogic = new JobItem_UI();
            newJobEntry.userData = newJobLogic;
            newJobLogic.SetVisualElement(newJobEntry);
            newJobLogic.SetData(job, JobImages[(int)(Random.value * JobImages.Length)], controller);
            job_ListView.Add(newJobEntry);
        }
    }
}

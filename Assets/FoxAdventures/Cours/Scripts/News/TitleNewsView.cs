using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

[System.Serializable]
public class TitleNewsViewEntry
{
public string title;
public string body;
public DateTime displayedDate;
public string DisplayedDateStr
{
    get
    {
        return this.displayedDate.ToLongDateString();
    }
}
}

public class TitleNewsView : MonoBehaviour
{
public Transform contentRoot = null; // Racine du contenu
public Text contentText = null; // Texte du contenu
public bool automaticShowLatestNews = false; // Afficher automatiquement les dernières nouvelles
void OnEnable()
{
    this.HideView();    // Cacher la vue par défaut

    if (this.automaticShowLatestNews == true)  // Afficher les dernières nouvelles si l'option est activée
        this.ShowLatestNews();
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape) == true)    // Cacher la vue si la touche Escape est appuyée
    {
        this.HideView();
    }
}

public void ShowLatestNews()
{
    if (PlayfabAuth.IsLoggedIn == true)    // Afficher les nouvelles si l'utilisateur est connecté
    {
        PlayFabClientAPI.GetTitleNews(new GetTitleNewsRequest(), OnGetTitleNewsSuccess, OnGetTitleNewsError);
    }
}

private void OnGetTitleNewsSuccess(GetTitleNewsResult result)
{
    List<TitleNewsViewEntry> news = new List<TitleNewsViewEntry>();

    foreach (var item in result.News)
    {
        DateTime dateTime = item.Timestamp.ToLocalTime();
        news.Add(new TitleNewsViewEntry()
        {
            title = item.Title,
            body = item.Body,
            displayedDate = dateTime
        });
    }

    if (news != null && news.Count > 0) // Afficher les nouvelles si au moins une nouvelle est trouvée
    {
        if (this.contentText != null)
        {
            string newsContent = string.Empty;
            for (int i = 0; i < news.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(newsContent) == false)
                    newsContent += "\n\n";

                newsContent += "- " + news[i].DisplayedDateStr + " -";
                newsContent += "\n<color=orange>" + news[i].title + "</color>";
                newsContent += "\n" + news[i].body;
            }

            this.contentText.text = newsContent; // Mettre à jour le contenu de la vue
        }

        this.ShowView();    // Afficher la vue
    }
    else    // Cacher la vue s'il n'y a aucune nouvelle
    {
        this.HideView();
    }
}

private void OnGetTitleNewsError(PlayFabError error)
{
    Debug.LogError("TitleNewsView.OnGetTitleNewsError() - Error: " + error.GenerateErrorReport());
    this.HideView();    // Cacher la vue en cas d'erreur
}

public void ShowView()  // Afficher la racine du contenu
{
    if (this.contentRoot != null)
        this.contentRoot.gameObject.SetActive(true);
}

public void HideView()  // Cacher la racine du contenu
{
    if (this.contentRoot != null)
        this.contentRoot.gameObject.SetActive(false);
}
}
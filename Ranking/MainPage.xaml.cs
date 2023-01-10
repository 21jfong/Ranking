using static Ranking.RankingClass;
using System.Text.RegularExpressions;
using System.Text;

namespace Ranking;

public partial class MainPage : ContentPage
{
    private RankingClass ranking;
    private string saveFilePath;

    public MainPage()
    {
        InitializeComponent();

        ranking = new RankingClass();
        if (ranking.IsPosEnabled)
        {
            PositionContentEntry.IsEnabled = true;
            PositionContentEntry.IsVisible = true;
            PosLabel.IsVisible = true;
        }
    }
    private void OnConfirmButtonClicked(Object sender, EventArgs e)
    {
        ConfirmButton.IsEnabled = false;
        Player p = new();

        //Set the values to the player
        try
        {
            if (winsContentEntry.Text is not null)
                p.Wins = int.Parse(winsContentEntry.Text);

            if (lossesContentEntry.Text is not null)
                p.Losses = int.Parse(lossesContentEntry.Text);

            if (ranking.IsPosEnabled)
                if (PositionContentEntry.Text is not null)
                    p.Position = int.Parse(PositionContentEntry.Text);
        }
        catch
        {
            DisplayAlert("Error", "Error setting player values, please check your values and try again.", "Ok");
            ConfirmButton.IsEnabled = true;
            return;
        }

        p.Name = nameContentEntry.Text;

        p.Description = descriptionContentEntry.Text;

        try
        {
            ranking.AddPlayer(p);
            vertLayout.Add(new Label());
        }
        catch (ArgumentException)
        {
            DisplayAlert("Duplicate Name", "This player has already been added", "Ok");
        }

        ConfirmButton.IsEnabled = true;
        ranking.Changed = true;
        DisplayPlayers(ranking);
    }

    private void DisplayPlayers(RankingClass players)
    {

        for (int i = 0; i < vertLayout.Count; i++)
        {
            if (vertLayout[i].GetType().Equals(typeof(Label)))
            {
                Label l = vertLayout[i] as Label;
                try
                {
                    l.Text = players.GetPlayerAtRank(i).Name + " Wins: " + players.GetPlayerAtRank(i).Wins;
                }
                catch
                {
                    return;
                }
            }
        }

        if (ranking.Changed)
            HasBeenSavedLabel.Text = "Unsaved Changes";
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        EditButton.IsVisible = false;

        // Search for player
        Player p;
        try
        {
            // Get the player if the name matches the search
            string text = EditContentEntry.Text;
            if (text is not null && !text.Equals(""))
                p = ranking.GetPlayer(text);
            else
            {
                await DisplayAlert("Edit Player", "Please enter a player name", "Ok");
                EditButton.IsVisible = true;
                return;
            }
        }
        catch
        {
            await DisplayAlert("Edit Player", "Error Finding Specified Player", "Ok");
            EditButton.IsVisible = true;
            return;
        }

        string editAction;
        if (ranking.IsPosEnabled == false)
        {
            editAction = await DisplayActionSheet("Edit " + p.Name + " (Pick an Option)", "Cancel", "Delete Player",
                "Name",
                "Description",
                "Wins",
                "Losses");
        }
        else
        {
            editAction = await DisplayActionSheet("Edit " + p.Name + " (Pick an Option)", "Cancel", "Delete Player",
                "Name",
                "Description",
                "Wins",
                "Losses",
                "Position");
        }

        if (editAction is not null)
        {
            try
            {
                if (editAction.Equals("Delete Player"))
                {
                    ranking.removePlayer(p);
                    vertLayout.RemoveAt(vertLayout.Count - 1);
                }
                if (editAction.Equals("Description"))
                {
                    string desc = await DisplayPromptAsync("Description", "Description");
                    if (desc is not null && !p.Name.Equals(""))
                        p.Description = desc;
                }
                if (editAction.Equals("Name"))
                {
                    string name = await DisplayPromptAsync("Name", "Name");
                    if (name is not null && !p.Name.Equals(""))
                        p.Name = name;
                }
                if (editAction.Equals("Wins"))
                {
                    string wins = await DisplayPromptAsync("Wins", "Wins");
                    if (wins is not null && !p.Wins.Equals(""))
                        p.Wins = int.Parse(wins);
                }
                if (editAction.Equals("Losses"))
                {
                    string losses = await DisplayPromptAsync("Losses", "Losses");
                    if (losses is not null && !p.Losses.Equals(""))
                        p.Losses = int.Parse(losses);
                }
                if (editAction.Equals("Position"))
                {
                    string position = await DisplayPromptAsync("Position", "Position");
                    if (position is not null && !p.Position.Equals(""))
                        p.Position = int.Parse(position);
                }

                ranking.Changed = true;
            }
            catch
            {
                await DisplayAlert("Edit Error", "Please check value and try again.", "Ok");
            }

            DisplayPlayers(ranking);
        }
        EditButton.IsVisible = true;
    }

    private async void OnViewButtonClicked(object sender, EventArgs e)
    {
        ViewButton.IsVisible = false;

        // Search for player
        Player p;
        try
        {
            // Get the player if the name matches the search
            string text = ViewPlayerEntry.Text;
            if (text is not null && !text.Equals(""))
                p = ranking.GetPlayer(text);
            else
            {
                // If the entry box is empty
                await DisplayAlert("View Player", "Please enter a player name", "Ok");
                ViewButton.IsVisible = true;
                return;
            }
        }
        catch
        {
            await DisplayAlert("View Player", "Error Finding Specified Player", "Ok");
            ViewButton.IsVisible = true;
            return;
        }

        if (ranking.IsPosEnabled)
            await DisplayAlert("Viewing " + p.Name, "Description: " + p.Description + "\n" + "Wins: " + p.Wins + "\n" + "Losses: " + p.Losses + "\n" + "Position: " + p.Position, "Ok");
        else
            await DisplayAlert("Viewing " + p.Name, "Description: " + p.Description + "\n" + "Wins: " + p.Wins + "\n" + "Losses: " + p.Losses, "Ok");
        ViewButton.IsVisible = true;
    }

    private void OnRosterButtonClicked(object sender, EventArgs e)
    {
        StringBuilder sb = new();
        int pos = 1;
        if (ranking.IsPosEnabled)
        {
            foreach (Player p in ranking.GetAllPlayersByPos())
            {
                sb.Append("Position " + p.Position.ToString() + ": " + p.Name + "\n\t\t Wins: " + p.Wins + " Losses: " + p.Losses + "\n");
            }
        }
        else
        {
            foreach (Player p in ranking.GetAllPlayers())
            {
                sb.Append("Position " + pos.ToString() + ": " + p.Name + "\n\t\t Wins: " + p.Wins + " Losses: " + p.Losses + "\n");
                pos++;
            }
        }
        DisplayAlert("Roster", sb.ToString(), "Ok");
    }

    private void PosClicked(object sender, EventArgs e)
    {
        ranking.IsPosEnabled = true;
        PositionContentEntry.IsEnabled = true;
        PositionContentEntry.IsVisible = true;
        PosLabel.IsVisible = true;
    }

    private void DisPosClicked(object sender, EventArgs e)
    {
        ranking.IsPosEnabled = false;
        PositionContentEntry.IsEnabled = false;
        PositionContentEntry.IsVisible = false;
        PosLabel.IsVisible = false;
    }

    private async void OnClearPlayersButtonClicked(object sender, EventArgs e)
    {
        bool isClear = await DisplayAlert("Clear All Players", "THIS WILL DELETE ALL PLAYERS", "Ok", "Cancel");

        if (isClear)
        {
            ranking.clearPlayers();
            vertLayout.Clear();
            await DisplayAlert("Clear All Players", "Players have been deleted", "Ok");
            ranking.Changed = true;
            DisplayPlayers(ranking);
        }
    }

    private void AddWinsButtonClicked(object sender, EventArgs e)
    {
        // Search for player
        Player p;
        try
        {
            // Get the player if the name matches the search
            string text = AddWinsEntry.Text;
            if (text is not null && !text.Equals(""))
                p = ranking.GetPlayer(text);
            else
            {
                DisplayAlert("Error", "Cannot find player", "Ok");
                return;
            }
        }
        catch
        {
            DisplayAlert("Error", "Cannot find player", "Ok");
            return;
        }

        if (p is not null)
        {
            p.Wins++;
            DisplayPlayers(ranking);
            ranking.Changed = true;
        }
    }

    private async void SaveAsClicked(object sender, EventArgs e)
    {
        saveFilePath = await DisplayPromptAsync("Save as", "FilePath:");
        if (saveFilePath is not null)
        {
            if (!File.Exists(saveFilePath))
            {
                // See if the file path is valid
                try
                {
                    ranking.Save(saveFilePath);
                    HasBeenSavedLabel.Text = "No Unsaved Changes";
                }
                catch (ArgumentException)
                {
                    await DisplayAlert("File Path Error", "There was a problem finding the file path.", "Ok");
                }
            }
            else
            {
                bool overwriteFile = await DisplayAlert("Overwrite Existing File", "The filepath already exists.", "Overwrite", "Cancel");
                if (overwriteFile)
                {
                    ranking.Save(saveFilePath);
                    HasBeenSavedLabel.Text = "No Unsaved Changes";
                }
            }
        }
        else
            await DisplayAlert("FilePath Error", "There was a problem saving to filepath: " + saveFilePath, "Ok");
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        if (saveFilePath is "" || saveFilePath is null)
            await DisplayAlert("Missing File Path", "You need to use \"save as\" before you can save.", "Ok");
        else
        {
            ranking.Save(saveFilePath);
            HasBeenSavedLabel.Text = "No Unsaved Changes";
        }
    }

    private async void OpenClicked(object sender, EventArgs e)
    {
        if (!ranking.Changed)
        {
            await OpenFileHelperAsync(sender, e);
        }
        else
        {
            if (await DisplayAlert("Unsaved changes", "You currently have unsaved changes.", "Continue", "Cancel"))
            {
                await OpenFileHelperAsync(sender, e);
            }
        }
    }

    private async Task OpenFileHelperAsync(Object sender, EventArgs e)
    {
        try
        {
            FileResult fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
                ranking = new(fileResult.FullPath);
                saveFilePath = fileResult.FullPath;

                for (int i = vertLayout.Count; i < ranking.Count; i++)
                    vertLayout.Add(new Label());

                if (ranking.IsPosEnabled)
                {
                    ranking.IsPosEnabled = true;
                    PositionContentEntry.IsEnabled = true;
                    PositionContentEntry.IsVisible = true;
                    PosLabel.IsVisible = true;
                }

                DisplayPlayers(ranking);
                await DisplayAlert("Open", "File has been opened.", "Ok");
                //undoStack.Clear();
            }
            else
            {
                await DisplayAlert("File not found", "Could not find file.", "Ok");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.ToString(), "Ok");
        }
    }

    private void PosHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("Positions", "Go to the settings tab and click either enable or disable, this setting will be remembered " +
            "the next time you open this file.", "Ok");
    }

    private void SaveHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("Saving", "Before saving, you must create an empty text file somewhere on your computer, after you have created it, provide the " +
            "file path to that text file to save as. After you have used \"save as\" once, you can save as many times as you want.", "Ok");
    }

    private void OpenHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("Opening", "Simply go to the settings tab and click open, where you will then search for the previously created text file you want to open.", "Ok");
    }

    private void DeleteHelpClicked(object sender, EventArgs e)
    {
        DisplayAlert("Deleting a player", "To delete a player you must enter the player's name into \"EditPlayer\" and scroll to the bottom of the prompt where it " +
            "gives you the option to delete the player.", "Ok");
    }
}


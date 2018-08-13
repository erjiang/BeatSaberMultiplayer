﻿using HMUI;
using SongLoaderPlugin.OverrideClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace BeatSaberMultiplayer.UI.ViewControllers.RoomScreen
{
    class VotingViewController : VRUIViewController, TableView.IDataSource
    {
        public event Action<CustomLevel> SongSelected;

        private Button _pageUpButton;
        private Button _pageDownButton;
        private TextMeshProUGUI _timerText;

        TableView _songsTableView;
        StandardLevelListTableCell _songTableCellInstance;

        List<CustomLevel> availableSongs = new List<CustomLevel>();

        protected override void DidActivate(bool firstActivation, ActivationType type)
        {
            if (firstActivation && type == ActivationType.AddedToHierarchy)
            {
                _songTableCellInstance = Resources.FindObjectsOfTypeAll<StandardLevelListTableCell>().First(x => (x.name == "StandardLevelListTableCell"));

                _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), rectTransform, false);
                (_pageUpButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 1f);
                (_pageUpButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 1f);
                (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -14f);
                _pageUpButton.interactable = true;
                _pageUpButton.onClick.AddListener(delegate ()
                {
                    _songsTableView.PageScrollUp();

                });
                _pageUpButton.interactable = false;

                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), rectTransform, false);
                (_pageDownButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0f);
                (_pageDownButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0f);
                (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 8f);
                _pageDownButton.interactable = true;
                _pageDownButton.onClick.AddListener(delegate ()
                {
                    _songsTableView.PageScrollDown();

                });
                _pageDownButton.interactable = false;

                _songsTableView = new GameObject().AddComponent<TableView>();
                _songsTableView.transform.SetParent(rectTransform, false);

                Mask viewportMask = Instantiate(Resources.FindObjectsOfTypeAll<Mask>().First(), _songsTableView.transform, false);
                viewportMask.transform.DetachChildren();
                _songsTableView.GetComponentsInChildren<RectTransform>().First(x => x.name == "Content").transform.SetParent(viewportMask.rectTransform, false);

                (_songsTableView.transform as RectTransform).anchorMin = new Vector2(0.3f, 0.5f);
                (_songsTableView.transform as RectTransform).anchorMax = new Vector2(0.7f, 0.5f);
                (_songsTableView.transform as RectTransform).sizeDelta = new Vector2(0f, 60f);
                (_songsTableView.transform as RectTransform).position = new Vector3(0f, 0f, 2.4f);
                (_songsTableView.transform as RectTransform).anchoredPosition = new Vector3(0f, -3f);

                ReflectionUtil.SetPrivateField(_songsTableView, "_pageUpButton", _pageUpButton);
                ReflectionUtil.SetPrivateField(_songsTableView, "_pageDownButton", _pageDownButton);

                _songsTableView.didSelectRowEvent += SongsTableView_DidSelectRow;
                _songsTableView.dataSource = this;

                _timerText = BeatSaberUI.CreateText(rectTransform, "", new Vector2(0f, -5f));
                _timerText.fontSize = 8f;
                _timerText.alignment = TextAlignmentOptions.Center;
                _timerText.rectTransform.sizeDelta = new Vector2(30f, 6f);
            }
            else
            {
                _songsTableView.ReloadData();
            }

        }

        protected override void LeftAndRightScreenViewControllers(out VRUIViewController leftScreenViewController, out VRUIViewController rightScreenViewController)
        {
            PluginUI.instance.roomFlowCoordinator.GetLeftAndRightScreenViewControllers(out leftScreenViewController, out rightScreenViewController);
        }

        private void SongsTableView_DidSelectRow(TableView sender, int row)
        {
            SongSelected?.Invoke(availableSongs[row]);
        }

        public void SetSongs(List<CustomLevel> levels)
        {
            availableSongs = levels;

            if (_songsTableView.dataSource != this)
            {
                _songsTableView.dataSource = this;
            }
            else
            {
                _songsTableView.ReloadData();
            }

            _songsTableView.ScrollToRow(0, false);
        }

        public void SetTimer(float time)
        {
            _timerText.text = SecondsToString(time);
        }

        public string SecondsToString(float time)
        {
            int minutes = (int)(time / 60f);
            int seconds = (int)(time - minutes * 60);
            return minutes.ToString() + ":" + string.Format("{0:00}", seconds);
        }

        public TableCell CellForRow(int row)
        {
            StandardLevelListTableCell cell = Instantiate(_songTableCellInstance);

            CustomLevel song = availableSongs[row];

            cell.coverImage = song.coverImage;
            cell.songName = $"{song.songName}\n<size=80%>{song.songSubName}</size>";
            cell.author = song.songAuthorName;

            cell.reuseIdentifier = "SongCell";

            return cell;
        }

        public int NumberOfRows()
        {
            return availableSongs.Count;
        }

        public float RowHeight()
        {
            return 10f;
        }
    }
}
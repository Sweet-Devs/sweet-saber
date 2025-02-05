﻿using System;
using System.Collections.Generic;
using SweetSaber.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ReactiveUI;
using SweetSaber.BeatMods;
using Avalonia;
using System.Reactive;
using DynamicData;
using SweetSaber.BeatMods.Models;
using System.Linq;

namespace SweetSaber.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static List<VerticalTab> Tabs => new()
        {
            new VerticalTab { Name = "Start" },
            new VerticalTab { Name = "Mods" },
            new VerticalTab { Name = "Settings" },
            new VerticalTab { Name = "About" },
        };

        private BeatModsAPI _modsAPI;
        private ObservableCollection<string> _versions;
        private ObservableCollection<SweetMod> _mods;
        private bool _filterPopupVisible;
        private int _tabIndex = 1;
        private string? _status;
        private string? _searchQuery;


        public string ApplicationVersion => App.VerString;
        public ObservableCollection<string> Versions
        {
            get => _versions;
            set => this.RaiseAndSetIfChanged(ref _versions, value);
        }
        
        public ObservableCollection<SweetMod> Mods
        {
            get => _mods;
            set => this.RaiseAndSetIfChanged(ref _mods, value);
        }
        public bool FilterPopupVisible
        {
            get => _filterPopupVisible;
            set => this.RaiseAndSetIfChanged(ref _filterPopupVisible, value);
        }

        public int TabIndex
        {
            get => _tabIndex;
            set => this.RaiseAndSetIfChanged(ref _tabIndex, value);
        }

        public string? Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
        
        public string? SearchQuery
        {
            get => _searchQuery;
            set => this.RaiseAndSetIfChanged(ref _searchQuery, value);
        }

        

        public MainWindowViewModel()
        {
            _modsAPI = new BeatModsAPI();
            _versions = new ObservableCollection<string>();
            _mods = new ObservableCollection<SweetMod>();
        }


        public async void LoadData(object? s, EventArgs e)
        {
            Status = "Loading versions...";

            var versions = await _modsAPI.GetVersions();
            Versions.Clear();
            Versions.AddRange(versions ?? new());

            if (Versions.Count == 0)
            {
                Status = "Error loading versions...";
                return;
            }

            Status = "Loading mods...";

            //TODO: Rework select into merging into mods loaded from disk
            var mods = (await _modsAPI.GetMods(Versions[0])).Select(x => new SweetMod
            {
                IsInstalled = false,
                UpdateAvailable = false,
                CurrentVersion = null,
                CurrentGameVersion = null,
                InstalledPath = null,
                Mod = x
            }).ToList();

            Mods.Clear();
            Mods.AddRange(mods);

            Status = $"Loaded {mods?.Count} mods";
        }

        public void TogglePopup()
        {
            FilterPopupVisible = !FilterPopupVisible;
        }

        public void ApplyModAction(SweetMod mod)
        {
            if (mod.Updating)
            {
                mod.Updating = false;
            }
            else
            {
                mod.Updating = true;
            }
        }
        
        public void ExpandMod(SweetMod mod)
        {
            mod.IsExpanded = !mod.IsExpanded;
        }
    }
}

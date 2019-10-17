﻿//************************  Notice  **********************************
//** This file is part of iS3
//**
//** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
//**
//** This library is free software; you can redistribute it and/or
//** modify it under the terms of the GNU Lesser General Public
//** License as published by the Free Software Foundation; either
//** version 3 of the License, or (at your option) any later version.
//**
//** This library is distributed in the hope that it will be useful,
//** but WITHOUT ANY WARRANTY; without even the implied warranty of
//** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//** Lesser General Public License for more details.
//**
//** In addition, as a special exception,  that plugins developed for iS3,
//** are allowed to remain closed sourced and can be distributed under any license .
//** These rights are included in the file LGPL_EXCEPTION.txt in this package.
//**
//**************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

using iS3.Core;

namespace iS3.Desktop
{
    /// <summary>
    /// Interaction logic for ObjectView.xaml
    /// </summary>

    public enum ObjectViewType { ChartView, TableView, TextView };

    public partial class ObjectView : UserControl
    {
        Dictionary<string, IEnumerable<DGObject>> _saved;
        TabControl _tabControlSaved;

        public ObjectView()
        {
            InitializeComponent();
            setViewType(ObjectViewType.ChartView);

            SizeChanged += ObjectView_SizeChanged;
        }

        void ObjectView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_saved != null)
                refreshChartView(_saved);
        }
        // see IS3DataGrid for more details
        private void dataGrid0_AutoGeneratingColumn(object sender,
            DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Graphics" ||
                e.PropertyName == "Attributes" ||
                e.PropertyName == "IsSelected" ||
                e.PropertyName == "OBJECTID" ||
                e.PropertyName == "SHAPE" ||
                e.PropertyName == "Shape" ||
                e.PropertyName == "SHAPE_Length" ||
                e.PropertyName == "Shape_Length" ||
                e.PropertyName == "SHAPE_Area" ||
                e.PropertyName == "Shape_Area"
                )
            {
                e.Cancel = true;
                return;
            }
        }
        // see IS3DataGrid for more details
        private void dataGrid0_AutoGeneratedColumns(object sender, EventArgs e)
        {
            if (dataGrid0.Columns.Count == 0)
                return;

            try
            {
                DataGridColumn col =
                    dataGrid0.Columns.FirstOrDefault(
                    c => c.Header.ToString() == "ID");
                if (col != null)
                    col.DisplayIndex = 0;

                col = dataGrid0.Columns.FirstOrDefault(
                    c => c.Header.ToString() == "Name");
                if (col != null)
                    col.DisplayIndex = 1;

                col = dataGrid0.Columns.FirstOrDefault(
                    c => c.Header.ToString() == "FullName");
                if (col != null)
                    col.DisplayIndex = 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            foreach (DataGridColumn _iter in dataGrid0.Columns)
            {
                //_iter.MaxWidth = 300;
            }
        }

        public void setViewType(ObjectViewType type)
        {
            if (type == ObjectViewType.ChartView)
            {
                chartViewGrid.Visibility = System.Windows.Visibility.Visible;
                tableViewGrid.Visibility = System.Windows.Visibility.Collapsed;
                textViewGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (type == ObjectViewType.TableView)
            {
                chartViewGrid.Visibility = System.Windows.Visibility.Collapsed;
                tableViewGrid.Visibility = System.Windows.Visibility.Visible;
                textViewGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (type == ObjectViewType.TextView)
            {
                chartViewGrid.Visibility = System.Windows.Visibility.Collapsed;
                tableViewGrid.Visibility = System.Windows.Visibility.Collapsed;
                textViewGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void chartViewButton_Click(object sender, RoutedEventArgs e)
        {
            setViewType(ObjectViewType.ChartView);
        }
        private void tableViewButton_Click(object sender, RoutedEventArgs e)
        {
            setViewType(ObjectViewType.TableView);
        }
        private void textViewButton_Click(object sender, RoutedEventArgs e)
        {
            setViewType(ObjectViewType.TextView);
        }
        private void largeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_saved == null || _saved.Count != 1)
                return;
            IEnumerable<DGObject> objs = _saved.Values.First();
            string names = string.Empty;
            DGObject firstObj = objs.First();
            foreach (DGObject obj in objs)
                names += obj.Name + ",";

            Window window = new Window();
            window.Width = SystemParameters.PrimaryScreenWidth /2 ;
            window.Height = SystemParameters.PrimaryScreenHeight /2 ;
            window.Owner = App.Current.MainWindow;
            window.WindowState = WindowState.Maximized;
            window.Title = "Monitoring data chart: " + names;
            window.SizeChanged +=  (o, args) =>
                {
                    List<FrameworkElement> chartViews = firstObj.chartViews(objs,
                        window.ActualWidth, window.ActualHeight - 40.0);

                    TabControl tabControl = new TabControl();
                    foreach (var chart in chartViews)
                    {
                        TabItem item = new TabItem();
                        item.Header = chart.Name;
                        tabControl.Items.Add(item);
                        item.Content = chart;
                    }
                    
                    if (_tabControlSaved == null)
                        tabControl.SelectedIndex = chartViewHolder.SelectedIndex;
                    else
                        tabControl.SelectedIndex = _tabControlSaved.SelectedIndex;
                    _tabControlSaved = tabControl;  // save it to temporary var
                    
                    Grid grid = new Grid();
                    grid.Children.Add(tabControl);
                    window.Content = grid;
                };
            window.Show();
        }

        // Summary:
        //     Object selection event listener.
        public void objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e)
        {
            // get current selected objects
            Dictionary<string, IEnumerable<DGObject>> selectedObjsDict =
                Globals.project.getSelectedObjs();
            // process added objs
            if (e.addedObjs != null)
            {
                foreach (string key in e.addedObjs.Keys)
                {
                    if (selectedObjsDict.ContainsKey(key))
                    {
                        IEnumerable<DGObject> objs = selectedObjsDict[key];
                        List<DGObject> objlist = objs.ToList();

                        IEnumerable<DGObject> objsToBeAdded = e.addedObjs[key];
                        foreach (var obj in objsToBeAdded)
                        {
                            int findIndex = -1;
                            // use ID to find the old one
                            for (int i=0;i< objlist.Count; i++)
                            {
                                if (objlist[i].ID == obj.ID)
                                {
                                    findIndex = i;
                                }
                            }
                            if (findIndex == -1)
                            {
                                objlist.Add(obj);
                            }
                            else
                            {
                                objlist[findIndex] = obj;
                            }
                        }

                        selectedObjsDict[key] = objlist.AsEnumerable();
                    }
                    else
                        selectedObjsDict.Add(key, e.addedObjs[key]);
                }
            }
            // process removed objects
            if (e.removedObjs != null)
            {
                foreach (string key in e.removedObjs.Keys)
                {
                    if (selectedObjsDict.ContainsKey(key))
                    {
                        IEnumerable<DGObject> objs = selectedObjsDict[key];
                        List<DGObject> objlist = objs.ToList();

                        IEnumerable<DGObject> objsToBeRemoved = e.removedObjs[key];
                        foreach (var obj in objsToBeRemoved)
                            //delete,use ID relate
                            objlist.Remove(objlist.Where(x=>x.ID== obj.ID).FirstOrDefault());

                        selectedObjsDict[key] = objlist.AsEnumerable();
                    }
                }
            }

            _saved = selectedObjsDict;

            // 
            refreshTextView(selectedObjsDict);
            //refreshTableView(selectedObjsDict);
            refreshChartView(selectedObjsDict);
        }

        void refreshTextView(
            Dictionary<string, IEnumerable<DGObject>> selectedObjsDict)
        {
            textViewHolder.Children.Clear();

            TextBlock text = new TextBlock();
            text.Text = "Object selections:";
            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            textViewHolder.Children.Add(text);

            foreach (string key in selectedObjsDict.Keys)
            {
                TextBlock block = new TextBlock();
                block.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                block.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                block.Text = string.Format(" Layer {0}: {1} selected.", key,
                    selectedObjsDict[key].Count());
                textViewHolder.Children.Add(block);
            }
        }

        void refreshTableView(
            Dictionary<string, IEnumerable<DGObject>> selectedObjsDict)
        {
            // If more than 1 type objects are selected, can't display it.
            if (selectedObjsDict.Count != 1)
            {
                tableViewNA.Visibility = System.Windows.Visibility.Visible;
                tableViewHolder.Visibility = System.Windows.Visibility.Collapsed;
                if (selectedObjsDict.Count == 0)
                    tableViewNA.Text = "No object selected.";
                else
                    tableViewNA.Text = "N.A. for current selections.";
                return;
            }

            // If no object are selected, just return.
            IEnumerable<DGObject> objs = selectedObjsDict.Values.First();
            if (objs.Count() == 0)
            {
                tableViewNA.Visibility = System.Windows.Visibility.Visible;
                tableViewHolder.Visibility = System.Windows.Visibility.Collapsed;
                tableViewNA.Text = "No object selected.";
                return;
            }

            // We can display the selected objects in datagrids
            tableViewNA.Visibility = System.Windows.Visibility.Collapsed;
            tableViewHolder.Visibility = System.Windows.Visibility.Visible;

            DGObject obj = objs.First();
            List<DataView> dataViews = obj.tableViews(objs);
            if (dataViews == null)
                return;

            // general info datagrid
            dataGrid0.ItemsSource = dataViews[0];
            // hide all other tabs at first
            for (int i=1; i<tableViewHolder.Items.Count; ++i)
            {
                TabItem item = tableViewHolder.Items[i] as TabItem;
                item.Visibility = System.Windows.Visibility.Collapsed;

                DataGrid datagrid = item.Content as DataGrid;
                if (datagrid != null)
                    datagrid.Visibility = System.Windows.Visibility.Collapsed;
            }

            // extra info in tabs with a datagrid in each tab
            for (int i=1; i<dataViews.Count; ++i)
            {
                TabItem item = null;
                DataGrid datagrid = null;
                if (tableViewHolder.Items.Count <= i)
                {
                    item = new TabItem();
                    tableViewHolder.Items.Add(item);

                    datagrid = new DataGrid();
                    datagrid.AutoGenerateColumns = true;
                    datagrid.CanUserAddRows = false;
                    datagrid.IsReadOnly = true;
                    datagrid.EnableRowVirtualization = true;
                    datagrid.AlternationCount = 2;

                    item.Content = datagrid;
                }
                else
                {
                    item = tableViewHolder.Items[i] as TabItem;
                    datagrid = item.Content as DataGrid;
                }
                item.Header = dataViews[i].Table.TableName;
                item.Visibility = System.Windows.Visibility.Visible;

                datagrid.Visibility = System.Windows.Visibility.Visible;
                datagrid.ItemsSource = dataViews[i];
            }

            if (tableViewHolder.SelectedIndex >= dataViews.Count
                || tableViewHolder.SelectedIndex == -1)
                tableViewHolder.SelectedIndex = 0;
        }

        void refreshChartView(
            Dictionary<string, IEnumerable<DGObject>> selectedObjsDict)
        {
            // If more than 1 type objects are selected, can't display it.
            if (selectedObjsDict.Count != 1)
            {
                chartViewNA.Visibility = System.Windows.Visibility.Visible;
                chartViewHolder.Visibility = System.Windows.Visibility.Collapsed;
                if (selectedObjsDict.Count == 0)
                    chartViewNA.Text = "No object selected.";
                else
                    chartViewNA.Text = "N.A. for current selections.";
                return;
            }

            // If no object are selected, just return.
            IEnumerable<DGObject> objs = selectedObjsDict.Values.First();
            if (objs.Count() == 0)
            {
                chartViewNA.Visibility = System.Windows.Visibility.Visible;
                chartViewHolder.Visibility = System.Windows.Visibility.Collapsed;
                chartViewNA.Text = "No object selected.";
                return;
            }

            // if no enough space then return
            if (objectViewGrid.ActualWidth < 10 ||
                objectViewGrid.ActualHeight < 60)
                return;

            DGObject obj = objs.First();
            List<FrameworkElement> chartViews = obj.chartViews(objs,
                objectViewGrid.ActualWidth, objectViewGrid.ActualHeight - 40.0);
            if (chartViews == null)
            {
                chartViewNA.Visibility = System.Windows.Visibility.Visible;
                chartViewHolder.Visibility = System.Windows.Visibility.Collapsed;
                chartViewNA.Text = "Not available for selected object.";
                return;
            }

            // We can display the selected objects in datagrids
            chartViewNA.Visibility = System.Windows.Visibility.Collapsed;
            chartViewHolder.Visibility = System.Windows.Visibility.Visible;
            int lastSelected = chartViewHolder.SelectedIndex;
            chartViewHolder.Items.Clear();

            foreach (var chart in chartViews)
            {
                TabItem item = new TabItem();
                item.Header = chart.Name;
                chartViewHolder.Items.Add(item);

                //ScrollViewer scrollViewer = new ScrollViewer();
                //scrollViewer.HorizontalScrollBarVisibility = 
                //    ScrollBarVisibility.Auto;
                //scrollViewer.VerticalScrollBarVisibility = 
                //    ScrollBarVisibility.Auto;

                //item.Content = scrollViewer;
                //scrollViewer.Content = chart;

                item.Content = chart;
            }

            // resume last select index if possible
            if (lastSelected == -1)
                chartViewHolder.SelectedIndex = 0;
            else if (lastSelected < chartViews.Count)
                chartViewHolder.SelectedIndex = lastSelected;

            if (chartViewHolder.SelectedIndex == -1)
                chartViewHolder.SelectedIndex = 0;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using SapereKindle.Dictionary;
using SapereKindle.TranslationAPI;

namespace SapereKindle
{
    public partial class Form1 : Form
    {
        private TranslationService _translationService;
        private DictionaryService _dictionaryService;
        private Random _random;
        private List<TranslationItem> _reviewItems = new List<TranslationItem>();
        private int _currentReviewIndex = -1;

        public Form1()
        {
            InitializeComponent();
            _random = new Random();
        }
        
        private void btnTranslate_Click(object sender, EventArgs e)
        {
            var translations = _translationService.TranslateTextAsync(tbInput.Text, "it", "en").GetAwaiter().GetResult();
            tbOutput.Text = translations[0];
            tbOutput2.Text = translations[1];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _translationService = new TranslationService();
            _dictionaryService = new DictionaryService();
            var dictionaries = _dictionaryService.GetAllDictionaries();
            cmbxDictionaries.DataSource = dictionaries;
            cmbxDictionaries.SelectedIndex = 0;
            cmbxReviewDictionary.DataSource = dictionaries;
            cmbxReviewDictionary.SelectedIndex = 0;
            GetNItems();
        }

        private void btnImportKindleMate_Click(object sender, EventArgs e)
        {
            var dictFile = (TranslationDictionaryFile)cmbxDictionaries.SelectedValue;
            var changed = _dictionaryService.ImportAsync(tbKindleMateFile.Text, dictFile).GetAwaiter().GetResult();
            MessageBox.Show("Finished. Changed=" + changed);
        }

        private void btnOpenKindleMateFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                tbKindleMateFile.Text = ofd.FileName;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        private void RefreshReviewPosition()
        {
            lReviewPosition.Text = $"{(_currentReviewIndex + 1)}/{_reviewItems.Count}";
            btnReviewPrevious.Enabled = _currentReviewIndex > 0;
            btnReviewNext.Enabled = _currentReviewIndex < _reviewItems.Count;
            if (_currentReviewIndex >= 0)
            {
                var reviewItem = _reviewItems[_currentReviewIndex];
                tbReviewSentence.Text = reviewItem.Sentence.Text;
                tbReviewWord.Text = reviewItem.Word.Text;
            }
            ClearReviewTranslation();
        }

        private void GetNItems()
        {            
            _reviewItems.Clear();
            for (int i = 0; i < Properties.Settings.Default.NumberOfItems; i++)
                GetNextReviewItem();
            _currentReviewIndex = 0;
            FillReviewWordsTab();
            RefreshReviewPosition();
        }

        private void GetNextReviewItem()
        {
            var currentDictFile = (TranslationDictionaryFile)cmbxReviewDictionary.SelectedValue;
            var dict = currentDictFile.Dictionary;
            TranslationItem reviewItem;
            do {
                var itemIndex = _random.Next(0, dict.Translations.Count);
                reviewItem = dict.Translations[itemIndex];
            } while (_reviewItems.Contains(reviewItem));
            _reviewItems.Add(reviewItem);
            _currentReviewIndex = _reviewItems.Count - 1;
            RefreshReviewPosition();
        }

        private void FillReviewWordsTab()
        {
            panelReviewWords.Controls.Clear();
            int y = 5;
            foreach(var item in _reviewItems)
            {
                var newWord = new WordReviewControl(item.Word.Text, item.Word.GoogleText);
                newWord.Location = new Point(5, y);
                panelReviewWords.Controls.Add(newWord);
                y += newWord.Height;
            }
        }
        private void btnReviewNew_Click(object sender, EventArgs e)
        {
            GetNextReviewItem();    
        }

        private void ClearReviewTranslation()
        {
            tbReviewTranslationSentenceAzure.Text = "";
            tbReviewTranslationSentenceGoogle.Text = "";
            tbReviewTranslationWordAzure.Text = "";
            tbReviewTranslationWordGoogle.Text = "";
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tbReviewWord_TextChanged(object sender, EventArgs e)
        {

        }

        private void rtbReviewSentence_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void tbReviewTranslationWordGoogle_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnReviewTranslate_Click(object sender, EventArgs e)
        {
            var reviewItem = _reviewItems[_currentReviewIndex];
            tbReviewTranslationWordAzure.Text = reviewItem.Word.AzureText;
            tbReviewTranslationWordGoogle.Text = reviewItem.Word.GoogleText;
            tbReviewTranslationSentenceAzure.Text = reviewItem.Sentence.AzureText;
            tbReviewTranslationSentenceGoogle.Text = reviewItem.Sentence.GoogleText;
        }

        private void tbReviewTranslationSentenceAzure_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnReviewRepeat_Click(object sender, EventArgs e)
        {
            ClearReviewTranslation();
        }

        private void btnReviewPrevious_Click(object sender, EventArgs e)
        {
            _currentReviewIndex--;
            RefreshReviewPosition();
        }

        private void btnReviewNext_Click(object sender, EventArgs e)
        {
            _currentReviewIndex++;
            RefreshReviewPosition();
        }

        private void btnGetSynonymsAndAntonyms_Click(object sender, EventArgs e)
        {
            var sa = SynonymsAntonymsUtil.GetAsync(tbReviewWord.Text).GetAwaiter().GetResult();
            var saForm = new FormSynonymsAntonyms(sa);
            saForm.ShowDialog();
        }

        private void tabPageReview_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void cmbxReviewDictionary_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPageReviewWords_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPageImport_Click(object sender, EventArgs e)
        {

        }

        private void btnReviewWordsNew_Click(object sender, EventArgs e)
        {
            GetNItems();
        }
    }
}

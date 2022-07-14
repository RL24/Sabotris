using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace Sabotris.Util
{
    public static class SteamLeaderboardsUtil
    {
        private const string LeaderBoardName = "Highscore"; 
        
        public static event EventHandler<LeaderboardFindResult_t> OnFindLeaderboardEvent;
        public static event EventHandler<LeaderboardScoresDownloaded_t> OnDownloadScoresEvent;
        public static event EventHandler<LeaderboardScoreUploaded_t> OnUploadScoreEvent;

        private static readonly CallResult<LeaderboardFindResult_t> FindLeaderboardCallResult;
        private static readonly CallResult<LeaderboardScoresDownloaded_t> ScoresDownloadedCallResult;
        private static readonly CallResult<LeaderboardScoreUploaded_t> ScoreUploadedCallResult;

        static SteamLeaderboardsUtil()
        {
            FindLeaderboardCallResult = CallResult<LeaderboardFindResult_t>.Create(OnFindLeaderboard);
            ScoresDownloadedCallResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnDownloadLeaderboard);
            ScoreUploadedCallResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnUploadedLeaderboardScore);
        }
        
        private static void OnFindLeaderboard(LeaderboardFindResult_t leaderboardFindResult, bool bIOFailure)
        {
            FindLeaderboardCallResult.Dispose();
            OnFindLeaderboardEvent?.Invoke(null, leaderboardFindResult);
        }

        private static void OnDownloadLeaderboard(LeaderboardScoresDownloaded_t leaderboardScoresDownloaded, bool bIOFailure)
        {
            ScoresDownloadedCallResult.Dispose();
            OnDownloadScoresEvent?.Invoke(null, leaderboardScoresDownloaded);
        }

        private static void OnUploadedLeaderboardScore(LeaderboardScoreUploaded_t leaderboardScoreUploaded, bool bIOFailure)
        {
            ScoreUploadedCallResult.Dispose();
            OnUploadScoreEvent?.Invoke(null, leaderboardScoreUploaded);
        }

        private static void FindLeaderboard()
        {
            FindLeaderboardCallResult.Set(SteamUserStats.FindLeaderboard(LeaderBoardName));
        }

        private static void DownloadScores(SteamLeaderboard_t leaderboard)
        {
            ScoresDownloadedCallResult.Set(SteamUserStats.DownloadLeaderboardEntries(leaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, 20));
        }

        private static void UploadScore(SteamLeaderboard_t leaderboard, int score)
        {
            ScoreUploadedCallResult.Set(SteamUserStats.UploadLeaderboardScore(leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, new int[0], 0));
        }

        public static IEnumerator GetLeaderboard(Atomic<SteamLeaderboard_t?> leaderboard)
        {
            void FoundLeaderboard(object sender, LeaderboardFindResult_t leaderboardFindResult)
            {
                OnFindLeaderboardEvent -= FoundLeaderboard;
                leaderboard.Value = leaderboardFindResult.m_hSteamLeaderboard;
            }

            OnFindLeaderboardEvent += FoundLeaderboard;
            FindLeaderboard();

            yield return new WaitUntil(() => leaderboard.Value != null);
        }

        public static IEnumerator GetLeaderboardScores(Atomic<List<LeaderboardEntry_t>> leaderboardEntries)
        {
            var foundLeaderboard = new Atomic<SteamLeaderboard_t?>(null);
            yield return GetLeaderboard(foundLeaderboard);
            
            if (foundLeaderboard.Value == null)
                yield break;

            var downloadedEntries = false;
            void DownloadedScores(object sender, LeaderboardScoresDownloaded_t leaderboardScoresDownloaded)
            {
                OnDownloadScoresEvent -= DownloadedScores;

                for (var i = 0; i < leaderboardScoresDownloaded.m_cEntryCount; i++)
                {
                    var details = new int[0];
                    SteamUserStats.GetDownloadedLeaderboardEntry(leaderboardScoresDownloaded.m_hSteamLeaderboardEntries, i, out var leaderboardEntry, details, 0);
                    leaderboardEntries.Value.Add(leaderboardEntry);
                }

                downloadedEntries = true;
            }

            OnDownloadScoresEvent += DownloadedScores;
            DownloadScores(foundLeaderboard.Value.Value);

            yield return new WaitUntil(() => downloadedEntries);
        }

        public static IEnumerator UploadLeaderboardScore(int score)
        {
            var foundLeaderboard = new Atomic<SteamLeaderboard_t?>(null);
            yield return GetLeaderboard(foundLeaderboard);

            if (foundLeaderboard.Value == null)
                yield break;
            
            UploadScore(foundLeaderboard.Value.Value, score);
        }
    }
}
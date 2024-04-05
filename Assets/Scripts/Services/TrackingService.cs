using System;

public class TrackingService
{
	private DateTime timeStartGame;
	private int gamePlayed;
	private GameMode mode;

	private FirebaseService firebaseService;

	public TrackingService(FirebaseService firebaseService)
	{
		this.firebaseService = firebaseService;
	}
	/// <summary>
	/// Tracking game start.
	/// </summary>
	/// <param name="game">order of game in game list</param>
	/// <param name="mode">mode of game played</param>
	public void StartGameTracking(int game, GameMode mode)
	{
		timeStartGame = DateTime.Now;
		gamePlayed = game;
		this.mode = mode;

		//firebaseService.LevelStart(game, mode);
	}
	/// <summary>
	/// Tracking game stop.
	/// </summary>
	public void StopGameTracking()
	{
		double timePlayed = (DateTime.Now - timeStartGame).TotalSeconds;

		firebaseService.SelectItem(gamePlayed, timePlayed);
	}
	/// <summary>
	/// Tracking first player win.
	/// </summary>
	public void TrackingRedWin()
	{
		double timePlayed = (DateTime.Now - timeStartGame).TotalSeconds;

		firebaseService.LevelComplete(gamePlayed, timePlayed, mode);
	}
	/// <summary>
	/// Tracking second player win.
	/// </summary>
	public void TrackingBlueWin()
	{
		double timePlayed = (DateTime.Now - timeStartGame).TotalSeconds;

		firebaseService.LevelFail(gamePlayed, timePlayed, mode);
	}
	// Tracking something
	public void TrackTransactionComplete(long maxStage, long userDay, string offerName, string offerPrice, string transactionPlacement)
	{
		firebaseService.TransactionComplete(maxStage, userDay, offerName, offerPrice, transactionPlacement);
	}

	public void TrackingScoreCombo(long maxStage, long userDay, long x2, long x3, long x4, long stage)
	{
		firebaseService.ScoreCombo(maxStage, userDay, x2, x3, x4, stage);
	}

	public void TrackingBooster(long gameMode, long maxStage, long userDay, long stage, long type0, long type1, long type2, long type3, string result, string reason)
	{
		firebaseService.Booster(gameMode, maxStage, userDay, stage, type0, type1, type2, type3, result, reason);
	}

	public void TrackingTutorialStep(long levelTutorial, long type0, long type1, long type2, long type3)
	{
		firebaseService.TutorialStep(levelTutorial, type0, type1, type2, type3);
	}

	public void TrackingLevelStart(long gameMode, long maxStage, long userDay, long stage)
	{
		firebaseService.LevelStart(gameMode, maxStage, userDay, stage);
	}

	public void TrackLevelEnd(long gameMode, long maxStage, long userDay, string result, long stage, long maxStreakLong, long totalScore, double completeTime, long numberRemaining)
	{
		firebaseService.LevelEnd(gameMode, maxStage, userDay, result, stage, maxStreakLong, totalScore, completeTime, numberRemaining);
	}
}

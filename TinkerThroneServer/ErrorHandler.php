<?php
abstract class ErrorHandler
{
	static $log_file = 'log.txt';

	public static function handle_warning(string $message)
	{
		var_dump('Server Side Warning!');
		
		$error_message = 'WARNING: ' . $message;
		if(isset($_SESSION) && isset($_SESSION['username']))
		{
			$error_message .= PHP_EOL . 'Caused by ' . $_SESSION['username'];
		}
		var_dump($error_message);
		
		if(file_exists(self::$log_file) && substr(sprintf('%o', fileperms(self::$log_file)), -3) >= 666)
		{
			error_log($error_message . PHP_EOL, 3, self::$log_file);
		}
		else
		{
			var_dump('No Log available!');
		}
	}
		
	public static function handle_error(string $message)
	{
		var_dump('Server Side Error!');
		
		$error_message = 'ERROR: ' . $message;
		if(isset($_SESSION) && isset($_SESSION['username']))
		{
			$error_message .= PHP_EOL . 'Caused by ' . $_SESSION['username'];
		}
		var_dump($error_message);
		
		if(file_exists(self::$log_file) && substr(sprintf('%o', fileperms(self::$log_file)), -3) >= 666)
		{
			error_log($error_message . PHP_EOL, 3, self::$log_file);
		}
		else
		{
			var_dump('No Log available!');
		}
		
		exit();
	}
}
?>
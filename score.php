<?php
	if(isset($_GET['addscore']) && $_GET['addscore'] == 1)
	{
		file_put_contents('scores.txt', $_GET['score'] . ":" . time() . ":". $_GET['name'] . PHP_EOL , FILE_APPEND | LOCK_EX);
		return;
	}

	$scores = [];
	$handle = fopen("scores.txt", "r");
	if ($handle)
	{
		while (($line = fgets($handle)) !== false)
		{
			$data = explode(':', $line, 3);
			$scores[] = [
				'score' => $data[0],
				'datetime' => date('Y-m-d H:i:s', $data[1]),
				'name' => $data[2]
			];
		}

		fclose($handle);
	}

	uasort($scores, function($a, $b) {
		return $b['score'] - $a['score'];
	});

	echo("
<html>
	<body>
		<table>
			<tr>
				<th>Name</th>
				<th>Date</th>
				<th>Score</th>
			</tr>
");
	foreach($scores as $score)
	{
		echo("
			<tr>
				<td>". $score['name'] . "</td>
				<td>". $score['datetime'] ."</td>
				<td>". $score['score'] ."</td>
			</tr>
			");
	}
	echo("
		</table>
	</body>
</html>");
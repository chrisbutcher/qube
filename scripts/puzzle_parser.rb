require 'json'

file = File.open("faq.txt", "r")
contents = file.read
file.close

contents_lines = contents.split("\n")
puts contents_lines[663]
puts contents_lines[49060]

# contents_lines = contents_lines[663]

puzzle_starting_points = contents.to_enum(:scan, /-+\n\d/m).map { Regexp.last_match }

extracted_puzzles = puzzle_starting_points.each_with_index.map do |puzzle_starting_point, i|
	current_puzzle = puzzle_starting_point
	next_puzzle = puzzle_starting_points[1 + 1]

	unless next_puzzle
		puts 'skipping last puzzle'
		next
	end

	start_of_current_puzzle = current_puzzle.offset(0).first
	end_of_current_puzzle = next_puzzle.offset(0).first - 1
	extracted_puzzle = contents[start_of_current_puzzle..end_of_current_puzzle]

	extracted_puzzle
end

puts extracted_puzzles.size

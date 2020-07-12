require 'bundler/inline'

gemfile do
  source 'https://rubygems.org'
  gem 'json', require: true
	gem 'pry'
	gem 'pry-byebug'
	gem 'fast_blank'
end

require 'json'

file = File.open("faq.txt", "r")
contents = file.read
file.close

contents.downcase!

# Santiziing and making input .txt file sane

contents.gsub!('rotations: 76/', 'rotations: 6/7')
contents.gsub!(
	'6x8 puzzle 211000-110121-120111-100112-100111                  rotations: 12',
	'6x8 puzzle 211000-110121-120111-100112-100111                  rotations: 8/12'
)
contents.gsub!(
	'7x8 puzzle 2110102                                             rotations: 15',
	'7x8 puzzle 2110102                                             rotations: 8/15'
)
contents.gsub!(
	'7x9 puzzle 1000111                                             rotations: 16',
	'7x9 puzzle 1000111                                             rotations: 9/16'
)

contents_lines = contents.split("\n")
puzzle_lines = contents_lines[664..49057]

puzzle_lines.reject! { |line| line.blank? }
puzzle_lines.reject! { |line| /^\s\s\s/ =~ line }
puzzle_lines.reject! { |line| /^-+/ =~ line }

puzzle_array = []
current_puzzle = []

puzzle_lines.each do |puzzle_line|
	start_of_puzzle = /rotations/ =~ puzzle_line || /-=-+/ =~ puzzle_line

	if start_of_puzzle
		if current_puzzle.size > 0
			puzzle_array << current_puzzle
			current_puzzle = []
		else
			current_puzzle = []
		end
	end

	current_puzzle << puzzle_line
end

puzzles = puzzle_array.map do |inner_puzzle_array|
	description_line = inner_puzzle_array[0]
	description = description_line.match(/^(.*?)\s+rotations/).captures.first

	width, height = description_line.match(/(\d+)x(\d+)/).captures

	guide_solved_in, typical_rotations_needed = description_line.match(/(\d+)\/(\d+|\?)/).captures
	typical_rotations_needed = guide_solved_in if typical_rotations_needed == '?'

	rows = rest_of_puzzle = inner_puzzle_array[1..-1].map do |puzz_line|
		cubes = puzz_line.to_enum(:scan, /\[([_*xX])\]+/).map { Regexp.last_match }.map do |cube_type|
			{ type: cube_type.captures.first } # Cube
		end

		{ row: cubes } # Row
	end

	# Puzzle
	{
		rows: rows,
		width: width,
		height: height,
		typical_rotations_needed: typical_rotations_needed,
		description: description
	}
end

puzzles_by_dimensions = puzzles.each_with_object({}) do |puzzle, memo|
	key = "#{puzzle[:width]}x#{puzzle[:height]}"
	memo[key] ||= []
	memo[key] << puzzle
end

output_file_path = '../Assets/StreamingAssets/puzzles.json'
File.write(output_file_path, puzzles_by_dimensions.to_json)

puts "Parsed and wrote puzzles to #{output_file_path} !"

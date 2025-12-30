class CreateUserDto {
  final String firstName;
  final String lastName;
  final String email;

  CreateUserDto({
    required this.firstName,
    required this.lastName,
    required this.email,
  });

  Map<String, dynamic> toJson() {
    return {
      'firstName': firstName,
      'lastName': lastName,
      'email': email,
    };
  }
}


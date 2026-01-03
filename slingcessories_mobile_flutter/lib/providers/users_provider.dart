import 'package:flutter/foundation.dart';
import '../models/user_dto.dart';
import '../services/api_service.dart';

class UsersProvider with ChangeNotifier {
  final ApiService _apiService = ApiService();
  List<UserDto> _users = [];
  bool _isLoading = false;
  String? _errorMessage;

  List<UserDto> get users => _users;
  bool get isLoading => _isLoading;
  String? get errorMessage => _errorMessage;

  Future<void> loadUsers() async {
    _isLoading = true;
    _errorMessage = null;
    notifyListeners();

    try {
      _users = await _apiService.getUsers();
    } catch (e) {
      _errorMessage = 'Failed to load users: $e';
      debugPrint(_errorMessage);
    } finally {
      _isLoading = false;
      notifyListeners();
    }
  }

  Future<void> deleteUser(String id) async {
    try {
      await _apiService.deleteUser(id);
      await loadUsers();
    } catch (e) {
      _errorMessage = 'Failed to delete user: $e';
      notifyListeners();
    }
  }
}


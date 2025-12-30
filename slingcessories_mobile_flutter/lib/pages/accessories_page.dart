import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/accessories_provider.dart';

class AccessoriesPage extends StatefulWidget {
  const AccessoriesPage({super.key});

  @override
  State<AccessoriesPage> createState() => _AccessoriesPageState();
}

class _AccessoriesPageState extends State<AccessoriesPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<AccessoriesProvider>().loadAccessories();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Accessories'),
        actions: [
          TextButton(
            onPressed: () {
              context.read<AccessoriesProvider>().loadAccessories();
            },
            child: const Text('All'),
          ),
          TextButton(
            onPressed: () {
              context.read<AccessoriesProvider>().loadAccessories(wishlist: true);
            },
            child: const Text('Wishlist'),
          ),
        ],
      ),
      body: Consumer<AccessoriesProvider>(
        builder: (context, provider, child) {
          if (provider.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          if (provider.errorMessage != null) {
            return Center(
              child: Text(
                provider.errorMessage!,
                style: const TextStyle(color: Colors.red),
              ),
            );
          }

          if (provider.accessories.isEmpty) {
            return const Center(child: Text('No accessories found'));
          }

          return ListView.builder(
            itemCount: provider.accessories.length,
            itemBuilder: (context, index) {
              final accessory = provider.accessories[index];
              return Card(
                margin: const EdgeInsets.all(8),
                child: ListTile(
                  leading: accessory.pictureUrl != null
                      ? Image.network(
                          accessory.pictureUrl!,
                          width: 80,
                          height: 80,
                          fit: BoxFit.cover,
                          errorBuilder: (context, error, stackTrace) {
                            return const Icon(Icons.image, size: 80);
                          },
                        )
                      : const Icon(Icons.image, size: 80),
                  title: Text(
                    accessory.title,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  subtitle: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(accessory.categoryName),
                      Text(
                        '\$${accessory.price.toStringAsFixed(2)}',
                        style: const TextStyle(
                          color: Colors.green,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      if (accessory.wishlist)
                        const Text(
                          'Wishlist',
                          style: TextStyle(color: Colors.orange),
                        ),
                    ],
                  ),
                  trailing: IconButton(
                    icon: const Icon(Icons.delete),
                    onPressed: () {
                      provider.deleteAccessory(accessory.id);
                    },
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}


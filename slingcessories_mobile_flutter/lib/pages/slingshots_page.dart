import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/slingshots_provider.dart';

class SlingshotsPage extends StatefulWidget {
  const SlingshotsPage({super.key});

  @override
  State<SlingshotsPage> createState() => _SlingshotsPageState();
}

class _SlingshotsPageState extends State<SlingshotsPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<SlingshotsProvider>().loadSlingshots();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Slingshots'),
      ),
      body: Consumer<SlingshotsProvider>(
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

          if (provider.slingshots.isEmpty) {
            return const Center(child: Text('No slingshots found'));
          }

          return ListView.builder(
            itemCount: provider.slingshots.length,
            itemBuilder: (context, index) {
              final slingshot = provider.slingshots[index];
              return Card(
                margin: const EdgeInsets.all(8),
                child: ListTile(
                  title: Text(
                    slingshot.displayName,
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  trailing: IconButton(
                    icon: const Icon(Icons.delete),
                    onPressed: () {
                      provider.deleteSlingshot(slingshot.id);
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


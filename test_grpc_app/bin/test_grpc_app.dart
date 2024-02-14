import 'package:grpc/grpc.dart';
import 'package:test_grpc_app/src/generated/greet.pbgrpc.dart';

Future<void> main(List<String> args) async {
  final channel = ClientChannel(
    'localhost', // Replace with your server's address
    port: 5001, // Replace with your server's port
    options: const ChannelOptions(credentials: ChannelCredentials.secure()),
  );

  final stub = GreeterClient(channel);

  try {
    final request = HelloRequest()..name = 'Shah Rukh Khan';
    final response = await stub.sayHello(request);

    for (var row in response.rows) {
      for (var column in row.columns) {
        print('${column.column}: ${column.value}');
      }
      print('-----------');
    }
  } catch (e) {
    print('Caught error: $e');
  }
  await channel.shutdown();
}

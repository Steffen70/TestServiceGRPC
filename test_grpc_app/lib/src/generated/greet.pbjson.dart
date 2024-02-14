//
//  Generated code. Do not modify.
//  source: greet.proto
//
// @dart = 2.12

// ignore_for_file: annotate_overrides, camel_case_types, comment_references
// ignore_for_file: constant_identifier_names, library_prefixes
// ignore_for_file: non_constant_identifier_names, prefer_final_fields
// ignore_for_file: unnecessary_import, unnecessary_this, unused_import

import 'dart:convert' as $convert;
import 'dart:core' as $core;
import 'dart:typed_data' as $typed_data;

@$core.Deprecated('Use helloRequestDescriptor instead')
const HelloRequest$json = {
  '1': 'HelloRequest',
  '2': [
    {'1': 'name', '3': 1, '4': 1, '5': 9, '10': 'name'},
  ],
};

/// Descriptor for `HelloRequest`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List helloRequestDescriptor = $convert.base64Decode(
    'CgxIZWxsb1JlcXVlc3QSEgoEbmFtZRgBIAEoCVIEbmFtZQ==');

@$core.Deprecated('Use tableReplyDescriptor instead')
const TableReply$json = {
  '1': 'TableReply',
  '2': [
    {'1': 'rows', '3': 1, '4': 3, '5': 11, '6': '.greet.TableRow', '10': 'rows'},
  ],
};

/// Descriptor for `TableReply`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List tableReplyDescriptor = $convert.base64Decode(
    'CgpUYWJsZVJlcGx5EiMKBHJvd3MYASADKAsyDy5ncmVldC5UYWJsZVJvd1IEcm93cw==');

@$core.Deprecated('Use tableRowDescriptor instead')
const TableRow$json = {
  '1': 'TableRow',
  '2': [
    {'1': 'columns', '3': 1, '4': 3, '5': 11, '6': '.greet.ColumnValue', '10': 'columns'},
  ],
};

/// Descriptor for `TableRow`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List tableRowDescriptor = $convert.base64Decode(
    'CghUYWJsZVJvdxIsCgdjb2x1bW5zGAEgAygLMhIuZ3JlZXQuQ29sdW1uVmFsdWVSB2NvbHVtbn'
    'M=');

@$core.Deprecated('Use columnValueDescriptor instead')
const ColumnValue$json = {
  '1': 'ColumnValue',
  '2': [
    {'1': 'column', '3': 1, '4': 1, '5': 9, '10': 'column'},
    {'1': 'value', '3': 2, '4': 1, '5': 9, '10': 'value'},
  ],
};

/// Descriptor for `ColumnValue`. Decode as a `google.protobuf.DescriptorProto`.
final $typed_data.Uint8List columnValueDescriptor = $convert.base64Decode(
    'CgtDb2x1bW5WYWx1ZRIWCgZjb2x1bW4YASABKAlSBmNvbHVtbhIUCgV2YWx1ZRgCIAEoCVIFdm'
    'FsdWU=');

